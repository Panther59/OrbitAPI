using Dapper.Contrib.Extensions;

namespace Orbit.Models.OrbitDB
{
	[Table("tblItemCodeSegments")]
	public class ItemCodeSegment : BaseAuditRecord
	{
		[ExplicitKey]
		public int? ID { get; set; }
		public int? OrganizationID { get; set; }
		public string Name { get; set; }
		public int? MaxLength { get; set; }
		public int? Sequence { get; set; }
	}
}
