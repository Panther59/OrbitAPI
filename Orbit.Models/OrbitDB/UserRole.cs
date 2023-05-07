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
		public int? CompanyID { get; set; }
		public int? ClientID { get; set; }
		public int? RoleID { get; set; }
		public int? UserID { get; set; }
		
		[Write(false)]
		public string? Company { get; set; }
		[Write(false)]
		public string? Client { get; set; }
		[Write(false)]
		public string? Role { get; set; }		
		[Write(false)]
		public string? User { get; set; }
	}
}
