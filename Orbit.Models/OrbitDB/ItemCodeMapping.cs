using Dapper.Contrib.Extensions;

namespace Orbit.Models.OrbitDB
{
	[Table("tblItemCodeMappings")]
	public class ItemCodeMapping : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ParentID { get; set; }
		public int? ChildID { get; set; }
	}
}
