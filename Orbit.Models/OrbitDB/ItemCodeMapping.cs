using Dapper.Contrib.Extensions;

namespace Orbit.Models.OrbitDB
{
	[Table("tblItemCodeMappings")]
	public class ItemCodeMapping : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ParentStructureID { get; set; }
		public int? ChildStructureID { get; set; }
	}
}
