using Dapper.Contrib.Extensions;

namespace Orbit.Models.OrbitDB
{
	[Table("tblItemCodes")]
	public class ItemCode : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ID { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public int? StructureID { get; set; }
	}
}
