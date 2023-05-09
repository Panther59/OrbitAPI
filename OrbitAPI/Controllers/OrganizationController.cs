using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.OrbitDB;
using System.Diagnostics;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class OrganizationController<T> : ControllerBase
		where T : Organization
	{
		private readonly IUserSession userSession;
		private readonly IOrganizationService<T> organizationService;

		public OrganizationController(IUserSession userSession, IOrganizationService<T> organizationService)
		{
			this.userSession = userSession;
			this.organizationService = organizationService;
		}

		[HttpGet]
		public Task<List<T>> GetAll()
		{
			if (this.userSession.HasPermission(Roles.Admin))
			{
				return this.organizationService.GetAllAsync();
			}
			else
			{
				if (!this.userSession.UserID.HasValue)
				{
					throw new Exception("You don't have permission to perform this action");
				}

				return this.organizationService.GetAllForUserAsync(this.userSession.UserID.Value);
			}
		}

		[HttpPut]
		public async Task<T> Upsert(T org)
		{
			var existingOrg = await this.organizationService.GetByNameAsync(org.Name);
			if (existingOrg != null)
			{
				if (!Debugger.IsAttached && !this.userSession.HasPermission(Roles.Admin, org.ID))
				{
					throw new Exception($"You do not have permission to update org record for {org.Name}, please reach out to Admin");
				}

				org.ID = existingOrg.ID;
				this.userSession.SetCreatedAuditColumns(org, existingOrg);
				this.userSession.SetUpdatedAuditColumns(org);

				await this.organizationService.UpdateAsync(org);

				return org;
			}
			else
			{
				if (!Debugger.IsAttached && !this.userSession.HasPermission(Roles.Admin))
				{
					throw new Exception($"You do not have permission to add new org, please reach out to Admin");
				}

				this.userSession.SetCreatedAuditColumns(org);
				this.userSession.SetUpdatedAuditColumns(org);
				org = await this.organizationService.AddAsync(org);

				return org;
			}
		}

		[HttpDelete("{id}")]
		public async Task Delete(int id)
		{
			var existingOrg = await this.organizationService.GetByIDAsync(id);
			if (existingOrg != null)
			{
				if (!this.userSession.HasPermission(Roles.Admin, existingOrg.ID))
				{
					throw new Exception($"You do not have permission to delete org record for {existingOrg.Name}, please reach out to Admin");
				}

				this.userSession.SetUpdatedAuditColumns(existingOrg);
				await this.organizationService.SetInactiveAsync(existingOrg);
			}
			else
			{
				throw new Exception($"Organization with id {id} doesn't exist, hence can't delete it");
			}
		}
	}
}
