﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Core.Exceptions;
using Orbit.Models.OrbitDB;

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

		[HttpPut("segments")]
		public async Task SaveItemCodeSegmentAsync(List<ItemCodeSegment> segments)
		{
			foreach (var segment in segments)
			{
				segment.OrganizationID = this.userSession.Organization?.ID;
				if (!segment.ID.HasValue)
				{
					/// New Segment
					this.userSession.SetCreatedAuditColumns(segment);
					this.userSession.SetUpdatedAuditColumns(segment);
					await this.itemCodeService.AddNewSegmentAsync(segment);
				}
				else
				{
					/// Existing segment
					this.userSession.SetUpdatedAuditColumns(segment);
					await this.itemCodeService.UpdateSegmentAsync(segment);
				}
			}
		}

		[HttpDelete("segments/{id}")]
		public async Task SaveItemCodeSegmentAsync(int id)
		{
			await this.itemCodeService.DeleteSegmentAsync(id);
			var currentList = await this.GetAll();
			var updateList = new List<ItemCodeSegment>();
			for (int i = 0; i < currentList.Count; i++)
			{
				if (currentList[i].Sequence != i + 1)
				{
					currentList[i].Sequence = i + 1;
					this.userSession.SetUpdatedAuditColumns(currentList[i]);
					updateList.Add(currentList[i]);
				}
			}

			if (updateList.Count > 0)
			{
				await this.SaveItemCodeSegmentAsync(updateList);
			}
		}
	}
}
