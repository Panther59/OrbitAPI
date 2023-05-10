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
	public class OrgsController : ControllerBase
	{
		private readonly IUserSession userSession;
		private readonly IOrganizationService organizationService;

		public OrgsController(IUserSession userSession, IOrganizationService organizationService)
		{
			this.userSession = userSession;
			this.organizationService = organizationService;
		}

		[HttpGet]
		public Task<List<Organization>> GetAll()
		{
			if (this.userSession.HasPermission(Permissions.ManageCompanies) || this.userSession.HasPermission(Permissions.ManageClients))
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

		private string OrgUpdatePermissionName(OrganizationType? type)
		{
			return type == OrganizationType.Client ? Permissions.ManageMyClientPermission : Permissions.ManageMyCompanyPermissions;
		}
		private string OrgUpdateAdminPermissionName(OrganizationType? type)
		{
			return type == OrganizationType.Client ? Permissions.ManageClients : Permissions.ManageCompanies;
		}

		[HttpPut]
		public async Task<Organization> Upsert(Organization org)
		{
			var existingOrg = await this.organizationService.GetByNameAsync(org.Name);
			if (existingOrg != null)
			{
				if (!Debugger.IsAttached &&
					(this.userSession.Organization != null &&
					this.userSession.Organization.ID == org.ID &&
					!this.userSession.HasPermission(this.OrgUpdatePermissionName(existingOrg.Type))) ||
					!this.userSession.HasPermission(this.OrgUpdateAdminPermissionName(existingOrg.Type)))
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
				if (!Debugger.IsAttached && !this.userSession.HasPermission(this.OrgUpdateAdminPermissionName(org.Type)))
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
				if (!Debugger.IsAttached &&
					(this.userSession.Organization != null &&
					this.userSession.Organization.ID == existingOrg.ID &&
					!this.userSession.HasPermission(this.OrgUpdatePermissionName(existingOrg.Type))) ||
					!this.userSession.HasPermission(this.OrgUpdateAdminPermissionName(existingOrg.Type)))
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
