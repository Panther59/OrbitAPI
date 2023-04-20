using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.OrbitDB;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
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
		public async Task<User> GetCurrentUser()
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
