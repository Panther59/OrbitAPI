using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.DTOs
{
	public class SegmentDetail : ItemCodeSegment
	{
		public List<ItemCode> Codes { get; set; }
		public SegmentDetail ParentSegment { get; set; }
		public SegmentDetail ChildSegment { get; set; }
		public List<ItemCodeMapping> Mappings { get; set; }
	}
}
