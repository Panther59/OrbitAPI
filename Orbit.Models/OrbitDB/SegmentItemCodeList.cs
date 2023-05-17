using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	public class SegmentItemCodeList : BaseAuditRecord
	{
		public int? SegmentID { get; set; }
		public int? SegmentPosition { get; set; }
		public int? ItemCodeListID { get; set; }
	}
}
