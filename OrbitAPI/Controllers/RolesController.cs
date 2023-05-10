using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Core.Exceptions;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class RolesController : ControllerBase
	{
		private readonly IUserSession userSession;
		private readonly IUserRoleService userRoleService;

		public RolesController(IUserSession userSession, IUserRoleService userRoleService)
		{
			this.userSession = userSession;
			this.userRoleService = userRoleService;
		}

		[HttpGet]
		public Task<List<Role>> GetAllRoles()
		{
			return this.userRoleService.GetAllRoles();
		}

		[HttpGet("users")]
		public Task<List<UserRole>> GetAllUserRoles([FromQuery] int? userId = null, [FromQuery] int? organizationId = null)
		{
			if (!userId.HasValue && !this.userSession.HasPermission(Permissions.ManageAllPermissions))
			{
				throw new Exception($"You do not have permission to view org list, please reach out to Admin");
			}

			return this.userRoleService.GetUserRoles(userId, organizationId);
		}

		[HttpGet("orgs")]
		public Task<List<Organization>> GetAllUserOrganizations([FromQuery] int? userId = null)
		{
			if (!userId.HasValue)
			{
				throw new BadRequestException("User ID is required");
			}

			return this.userRoleService.GetUserOrganizations(userId.Value);
		}

		[HttpPut("users")]
		public async Task<UserRole> AddRole(UserRole userRole)
		{
			if (!userRole.UserID.HasValue)
			{
				throw new BadRequestException("User details is missing");
			}

			if (userRole.UserID == this.userSession.UserID)
			{
				throw new BadRequestException("You can not change permission for your own account, reach out to other Admin user");
			}

			if (!userRole.RoleID.HasValue)
			{
				throw new BadRequestException("User Role details is missing");
			}

			userSession.SetCreatedAuditColumns(userRole);
			userSession.SetUpdatedAuditColumns(userRole);
			userRole = await this.userRoleService.AddRole(userRole);
			return userRole;
		}

		[HttpDelete]
		public async Task DeleteRole(UserRole userRole)
		{
			if (!userRole.UserID.HasValue)
			{
				throw new BadRequestException("User details is missing");
			}

			if (!userRole.RoleID.HasValue)
			{
				throw new BadRequestException("User Role details is missing");
			}

			await this.userRoleService.DeleteRole(userRole);
		}
	}
}
