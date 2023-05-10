using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Table("tblOrganizations")]
	public class Organization : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ID { get; set; }
		public string? Name { get; set; }
		public string? Code { get; set; }
		public OrganizationType? Type { get; set; }
		public string? AddressLine1 { get; set; }
		public string? AddressLine2 { get; set; }
		public string? City { get; set; }
		public string? ZipCode { get; set; }
		public string? Email { get; set; }
		public bool? IsActive { get; set; }
	}
}
