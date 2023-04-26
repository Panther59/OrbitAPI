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
		private readonly IUserService userService;

		public RolesController(IUserSession userSession, IUserService userService)
		{
			this.userSession = userSession;
			this.userService = userService;
		}

		[HttpGet]
		public Task<List<Role>> GetAllRoles()
		{
			return this.userService.GetAllRoles();
		}

		[HttpPut]
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

			userSession.SetUpdatedAuditColumns(userRole);
			userSession.SetUpdatedAuditColumns(userRole);
			userRole = await this.userService.AddRole(userRole);
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

			await this.userService.DeleteRole(userRole);
		}
	}
}
