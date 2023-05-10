using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Table("tblRoles")]
	public class Role : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ID { get; set; }
		public string Name { get; set; }		
		public RoleTypes Type { get; set; }
		public int? Ranking { get; set; }
	}
}
