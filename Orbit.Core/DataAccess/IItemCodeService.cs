using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IItemCodeService
	{
		Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment);
		Task DeleteSegmentAsync(int id);
		Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId);
		Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment);
	}
}