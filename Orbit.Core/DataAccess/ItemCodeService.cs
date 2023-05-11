using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class ItemCodeService : IItemCodeService
	{
		private readonly ISqlClient sqlClient;
		private readonly string itemCodeSegmentTableName;

		public ItemCodeService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
			this.itemCodeSegmentTableName =  this.sqlClient.TableNameMapper(typeof(ItemCodeSegment));	
		}

		public async Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment)
		{
			var maxId = await this.sqlClient.GetMaxID<ItemCodeSegment>();
			segment.ID = (maxId ?? 0) + 1;
			await this.sqlClient.InsertAsync(segment);
			return segment;
		}

		public async Task DeleteSegmentAsync(int id)
		{
			await this.sqlClient.DeleteAsync<ItemCodeSegment>(id);
		}

		public Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId)
		{
			var sql = $"SELECT * FROM {this.itemCodeSegmentTableName} WHERE OrganizationID = {orgId} ORDER BY Sequence";
			return this.sqlClient.GetData<ItemCodeSegment>(sql);
		}

		public async Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment)
		{
			await this.sqlClient.UpdateAsync(segment);
			return segment;
		}
	}
}
