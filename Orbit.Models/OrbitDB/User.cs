using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Table("tblUsers")]
	public class User
	{
		[Key]
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public DateTime Created { get; set; }
		public int CreatedBy { get; set; }
		public DateTime? Updated { get; set; }
		public int? UpdatedBy { get; set; }
	}
}
