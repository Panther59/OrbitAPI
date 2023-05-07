using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.OrbitDB;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly IUserSession userSession;
		private readonly IUserService userService;

		public UsersController(IUserSession userSession, IUserService userService)
		{
			this.userSession = userSession;
			this.userService = userService;
		}

		[HttpGet]
		public Task<List<User>> GetAllCurrentUser()
		{
			return this.userService.GetAllUsers();
		}

		[HttpGet("me")]
		public async Task<User> GetCurrentUser()
		{
			var userId = this.userSession.UserID;
			if (userId != null)
			{
				var user = await this.userService.GetUserByID(userId.Value);
				user.Picture = this.userSession.Picture;
				return user;
			}
			else
			{
				return null;
			}
		}

		[HttpGet("menu")]
		public async Task<User> GetCurrentUserMenus()
		{
			var userId = this.userSession.UserID;
			if (userId != null)
			{
				return await this.userService.GetUserByID(userId.Value);
			}
			else
			{
				return null;
			}
		}
	}
}
