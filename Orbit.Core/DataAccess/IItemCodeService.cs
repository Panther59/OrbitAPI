using Orbit.Models.OrbitDB;
using System.Data;

namespace Orbit.Core.DataAccess
{
	public interface IItemCodeService
	{
		Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment);
		Task DeleteSegmentAsync(int id);
		Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId);
		Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment);
		Task<List<string>> ValidateAndAddMappings(DataTable table, int parentId);
	}
}