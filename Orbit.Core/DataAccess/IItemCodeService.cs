using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using System.Data;

namespace Orbit.Core.DataAccess
{
	public interface IItemCodeService
	{
		Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment);
		Task DeleteMappingsAsync(List<ItemCodeMapping> mappings);
		Task DeleteSegmentAsync(int id);
		Task<SegmentDetail> GetItemCodeSegmentDetails(int id);
		Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId);
		Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment);
		Task<List<string>> ValidateAndAddMappings(DataTable table, int parentId);
	}
}