using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Table("tblUserRoles")]
	public class UserRole : BaseAuditRecord
	{
		public int? OrganizationID { get; set; }
		public int? RoleID { get; set; }
		public int? UserID { get; set; }

		[Write(false)]
		public RoleTypes? RoleType { get; set; }
		[Write(false)]
		public string? Email { get; set; }
		[Write(false)]
		public string? Organization { get; set; }
		[Write(false)]
		public string? Role { get; set; }
		[Write(false)]
		public string? User { get; set; }
		[Write(false)]
		public int? RoleRanking { get; set; }
	}
}
