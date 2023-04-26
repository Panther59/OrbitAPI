using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core
{
	public class UserSession : IUserSession
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public UserSession(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public int? UserID
		{
			get
			{
				var userId = httpContextAccessor.HttpContext.User?.Identity?.Name;
				if (!string.IsNullOrEmpty(userId))
				{
					return int.Parse(userId);
				}

				return null;
			}
		}

		public string? Picture
		{
			get
			{
				var identity = httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity;
				var pictureClaim = identity?.Claims.FirstOrDefault(x => x.Type == "AvatarUrl");
				if (pictureClaim != null && !string.IsNullOrEmpty(pictureClaim.Value))
				{
					return pictureClaim.Value;
				}

				return null;
			}
		}
	}
}
