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
		public Task<List<UserRole>> GetAllUserRoles([FromQuery] int? userId = null, [FromQuery] int? companyId = null, [FromQuery] int? clientId = null)
		{
			return this.userRoleService.GetUserRoles(userId, companyId, clientId);
		}

		[HttpPut("users")]
		public async Task<UserRole> AddRole(UserRole userRole)
		{
			if (!userRole.UserID.HasValue)
			{
				throw new BadRequestException("User details is missing");
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
