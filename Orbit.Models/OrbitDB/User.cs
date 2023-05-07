using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Dapper.Contrib.Extensions.Table("tblUsers")]
	public class User : BaseAuditRecord
	{
		[ExplicitKey]
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }

		[Computed]
		public string Name => $"{FirstName} {LastName}";
		[Computed]
		public string Initial => $"{FirstName?.Substring(0, 1)}{LastName.Substring(0, 1)}";
		[Write(false)]
		public string? Picture { get; set; }
		[Write(false)]
		public List<UserRole> Roles { get; set; }
	}
}
