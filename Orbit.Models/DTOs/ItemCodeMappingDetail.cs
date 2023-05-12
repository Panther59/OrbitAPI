using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.DTOs
{
	public class ItemCodeMappingDetail : ItemCodeMapping
	{
		public ItemCode Parent { get; set; }
		public ItemCode Child { get; set; }
	}
}
