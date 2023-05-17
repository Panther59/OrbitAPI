using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using OfficeOpenXml;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ItemCodesController : ControllerBase
	{
		private readonly IItemCodeService itemCodeService;
		private readonly IUserSession userSession;

		public ItemCodesController(
			IItemCodeService itemCodeService,
			IUserSession userSession)
		{
			this.itemCodeService = itemCodeService;
			this.userSession = userSession;
		}

		[HttpGet("lists/{listId}")]
		public async Task<List<ItemCode>> GetItemCodes(int listId, [FromQuery] int? parentId = null)
		{
			return await this.itemCodeService.GetItemCodesAsync(listId, parentId);
		}

		[HttpGet("lists")]
		public async Task<List<ItemCodeList>> GetItemCodeListsAsync()
		{
			return await this.itemCodeService.GetItemCodeListsAsync();
		}

		[HttpPut("lists")]
		public async Task<ItemCodeList> GetItemCodeListsAsync(ItemCodeList code)
		{
			return await this.itemCodeService.CreateItemCodeListsAsync(code);
		}

		[HttpPost("mappings/{parentId}"), DisableRequestSizeLimit]
		public async Task<List<string>> UploadMappingsExcel(int parentId)
		{
			var formCollection = await Request.ReadFormAsync();
			var files = formCollection.Files;
			try
			{
				var table = this.GetDataTableFromExcel(files.First());
				var results = await this.itemCodeService.ValidateAndAddMappings(table, parentId);
				return results;
			}
			catch (ValidationException ve)
			{
				var result = new List<string>() { ve.Message.ToString() };
				return result;
			}
			catch (Exception)
			{
				throw;
			}

		}

		[HttpPost("mappings/bulkDelete")]
		public async Task BulkDeleteMappings(List<ItemCodeMapping> mappings)
		{
			await this.itemCodeService.DeleteMappingsAsync(mappings);
		}

		private DataTable GetDataTableFromExcel(IFormFile formFile)
		{
			if (formFile.FileName.Split('.').Last() != "xls" && formFile.FileName.Split('.').Last() != "xlsx")
				throw new ValidationException("Please send an excel file to upload");

			var ms = new MemoryStream();
			formFile.CopyTo(ms);

			ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
			using (ExcelPackage package = new ExcelPackage(ms))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets[0];
				DataTable table = new DataTable();

				using (ExcelWorksheet workSheet = package.Workbook.Worksheets.First())
				{
					int noOfCol = workSheet.Dimension.End.Column;
					int noOfRow = workSheet.Dimension.End.Row;
					int rowIndex = 1;

					for (int c = 1; c <= noOfCol; c++)
					{
						table.Columns.Add(workSheet.Cells[rowIndex, c].Text);
					}
					rowIndex = 2;
					for (int r = rowIndex; r <= noOfRow; r++)
					{
						DataRow dr = table.NewRow();
						for (int c = 1; c <= noOfCol; c++)
						{
							dr[c - 1] = workSheet.Cells[r, c].Value;
						}
						table.Rows.Add(dr);
					}
				}

				return table;
			}

		}

		[HttpGet("{identifier}")]
		public async Task<SegmentDetail> GetSegmentCodeDetails(string identifier)
		{
			if (int.TryParse(identifier, out var id))
			{
				var result = await this.itemCodeService.GetItemCodeSegmentDetails(id);
				return result;
			}
			else
			{
				var result = await this.itemCodeService.GetItemCodeSegmentDetails(identifier);
				return result;
			}

		}

		[HttpPut]
		public async Task<ItemCode> updateItemCode(ItemCode itemCode)
		{
			if (itemCode.ID != null)
			{
				this.userSession.SetUpdatedAuditColumns(itemCode);
				var result = await this.itemCodeService.UpdateItemCodeAsync(itemCode);
				return result;
			}
			else
			{
				var dateTime = DateTime.Now;
				this.userSession.SetCreatedAuditColumns(itemCode, null, dateTime);
				this.userSession.SetUpdatedAuditColumns(itemCode, dateTime);
				var result = await this.itemCodeService.AddItemCodeAsync(itemCode);
				return result;
			}

		}

		[HttpPost("bulkDelete")]
		public async Task DeleteItemCode(List<ItemCode> items)
		{
			var updTime = DateTime.Now;
			foreach (var item in items)
			{
				item.IsActive = false;
				this.userSession.SetUpdatedAuditColumns(item, updTime);
			}

			await this.itemCodeService.DeleteItemCodeAsync(items);
		}
	}
}
