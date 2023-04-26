using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	public class UserRole : BaseAuditRecord
	{
		public int? CompanyID { get; set; }
		public string Company { get; set; }
		public int? ClientID { get; set; }
		public string Client { get; set; }
		public int? RoleID { get; set; }
		public string Role { get; set; }		
		public int? UserID { get; set; }
	}
}
