using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;

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
			await this.itemCodeService.DeleteItemCodeAsync(items);
		}
	}
}
