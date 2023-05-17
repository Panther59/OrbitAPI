using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Table("tblItemCodeList")]
	public class ItemCodeList : BaseAuditRecord
	{
		public int? ID { get; set; }
		public string Name { get; set; }
	}
}
