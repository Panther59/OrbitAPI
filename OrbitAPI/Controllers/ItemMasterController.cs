using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Core.Exceptions;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ItemMasterController : ControllerBase
	{
		private readonly IItemCodeService itemCodeService;
		private readonly IUserSession userSession;

		public ItemMasterController(
			IItemCodeService itemCodeService,
			IUserSession userSession)
		{
			this.itemCodeService = itemCodeService;
			this.userSession = userSession;
		}

		[HttpGet("segments")]
		public async Task<List<ItemCodeSegment>> GetAll()
		{
			if (this.userSession.Organization?.ID == null)
			{
				throw new BadRequestException("Organization detail is missing");
			}

			return await this.itemCodeService.GetSegmentsAsync(this.userSession.Organization.ID.Value);
		}

		[HttpGet("segments/{id}")]
		public async Task<List<ItemCodeSegment>> GetDetailById(int id)
		{
			if (this.userSession.Organization?.ID == null)
			{
				throw new BadRequestException("Organization detail is missing");
			}

			return await this.itemCodeService.GetSegmentsAsync(this.userSession.Organization.ID.Value);
		}

		[HttpPut("segments")]
		public async Task SaveItemCodeSegmentAsync(List<ItemCodeSegment> segments)
		{
			await this.PersistData(segments);
			await FixData();
		}

		[HttpDelete("segments/{id}")]
		public async Task SaveItemCodeSegmentAsync(int id)
		{
			await this.itemCodeService.DeleteSegmentAsync(id);
			await FixData();
		}

		private async Task PersistData(List<ItemCodeSegment> segments)
		{
			var updateTime = DateTime.Now;
			foreach (var segment in segments)
			{
				segment.OrganizationID = this.userSession.Organization?.ID;
				if (!segment.ID.HasValue)
				{
					/// New Segment
					this.userSession.SetCreatedAuditColumns(segment, null, updateTime);
					this.userSession.SetUpdatedAuditColumns(segment, updateTime);
					await this.itemCodeService.AddNewSegmentAsync(segment);
				}
				else
				{
					/// Existing segment
					this.userSession.SetUpdatedAuditColumns(segment, updateTime);
					await this.itemCodeService.UpdateSegmentAsync(segment);
				}
			}
		}

		private async Task FixData()
		{
			var updateTime = DateTime.Now;
			var currentList = await this.GetAll();
			var updateList = new List<ItemCodeSegment>();
			for (int i = 0; i < currentList.Count; i++)
			{
				bool dirty = false;
				if (i == 0)
				{
					if (currentList[i].ParentID.HasValue)
					{
						currentList[i].ParentID = null;
						dirty = true;
					}
				}
				else
				{
					if (currentList[i].ParentID != currentList[i - 1].ID)
					{
						currentList[i].ParentID = currentList[i - 1].ID;
						dirty = true;
					}
				}


				if (currentList[i].Sequence != i + 1)
				{
					currentList[i].Sequence = i + 1;
					dirty = true;
				}

				if (dirty)
				{
					this.userSession.SetUpdatedAuditColumns(currentList[i], updateTime);
					updateList.Add(currentList[i]);
				}
			}

			if (updateList.Count > 0)
			{
				await this.PersistData(updateList);
			}
		}
	}
}
