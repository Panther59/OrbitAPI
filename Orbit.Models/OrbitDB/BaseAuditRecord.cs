using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	public abstract class BaseAuditRecord
	{
		public DateTime? Created { get; set; }
		public int? CreatedBy { get; set; }
		public DateTime? Updated { get; set; }
		public int? UpdatedBy { get; set; }
	}
}
