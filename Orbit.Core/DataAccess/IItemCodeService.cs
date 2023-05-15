using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using System.Data;

namespace Orbit.Core.DataAccess
{
	public interface IItemCodeService
	{
		Task<ItemCode> AddItemCodeAsync(ItemCode itemCode);
		Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment);
		Task DeleteItemCodeAsync(List<ItemCode> items);
		Task DeleteMappingsAsync(List<ItemCodeMapping> mappings);
		Task DeleteSegmentAsync(int id);
		Task<SegmentDetail> GetItemCodeSegmentDetails(int id);
		Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId);
		Task<ItemCode> UpdateItemCodeAsync(ItemCode itemCode);
		Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment);
		Task<List<string>> ValidateAndAddMappings(DataTable table, int parentId);
	}
}