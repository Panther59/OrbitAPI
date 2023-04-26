using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

		public List<UserRole> Roles
		{
			get
			{
				var identity = httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity;
				var pictureClaim = identity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
				if (pictureClaim != null && !string.IsNullOrEmpty(pictureClaim.Value))
				{
					return JsonConvert.DeserializeObject<List<UserRole>>(pictureClaim.Value);
				}

				return null;
			}
		}

		public bool HasPermission(string role, int? companyId = null)
		{
			bool allow = false;
			var roles = this.Roles;
			if (roles != null)
			{
				foreach (var item in roles)
				{
					if (item.Role == role && ((companyId.HasValue && item.CompanyID == companyId) || (item.CompanyID == null && !companyId.HasValue)))
					{
						allow = true;
						break;
					}
				}
			}

			return allow;
		}

		public void SetUpdatedAuditColumns(BaseAuditRecord record)
		{
			record.Updated = DateTime.Now;
			record.UpdatedBy = this.UserID;
		}

		public void SetCreatedAuditColumns(BaseAuditRecord record, BaseAuditRecord? originalRecord = null)
		{
			record.Created = originalRecord?.Created ?? DateTime.Now;
			record.CreatedBy = originalRecord?.CreatedBy ?? this.UserID ?? 1;
		}
	}
}
