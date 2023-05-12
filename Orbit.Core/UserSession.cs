using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
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

		public Organization? Organization
		{
			get
			{
				var identity = httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity;
				var orgClaim = identity?.Claims.FirstOrDefault(x => x.Type == "Organization");
				if (orgClaim != null && !string.IsNullOrEmpty(orgClaim.Value))
				{
					return System.Text.Json.JsonSerializer.Deserialize<Organization>(orgClaim.Value);
				}

				return null;
			}
		}

		public List<string>? Permissions
		{
			get
			{
				var identity = httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity;
				var permissions = identity?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

				return permissions;
			}
		}

		public bool HasPermission(string role)
		{
			return httpContextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, role);
		}

		public void SetUpdatedAuditColumns(BaseAuditRecord record, DateTime? dateTime = null)
		{
			record.UpdatedOn = dateTime ?? DateTime.Now;
			record.UpdatedBy = this.UserID ?? 1;
		}

		public void SetCreatedAuditColumns(BaseAuditRecord record, DateTime? dateTime = null)
		{
			this.SetCreatedAuditColumns(record, null, dateTime);
		}

		public void SetCreatedAuditColumns(BaseAuditRecord record, BaseAuditRecord? originalRecord = null, DateTime? dateTime = null)
		{
			record.CreatedOn = originalRecord?.CreatedOn ?? dateTime ?? DateTime.Now;
			record.CreatedBy = originalRecord?.CreatedBy ?? this.UserID ?? 1;
		}
	}
}
