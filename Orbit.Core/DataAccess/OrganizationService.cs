using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class OrganizationService : IOrganizationService
	{
		private readonly ISqlClient sqlClient;
		private readonly string tableName;

		public ISqlClient SqlClient => sqlClient;

		public string TableName => tableName;

		public OrganizationService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
			var type = typeof(Organization);
			this.tableName = sqlClient.TableNameMapper(type);
			if (string.IsNullOrEmpty(this.tableName))
			{
				throw new Exception($"Unable to identify database table name for Type {type.FullName}");
			}
		}

		public async Task<Organization> AddAsync(Organization org)
		{
			var existingOrg = await this.GetByNameAsync(org.Name);
			if (existingOrg == null)
			{
				var maxId = await this.SqlClient.GetMaxID(TableName);
				org.ID = (maxId ?? 0) + 1;
				await this.SqlClient.InsertAsync(org);

				return org;
			}
			else
			{
				if (org.IsActive == true)
				{
					throw new Exception($"There is already a valid org in the system with name {org.Name}");
				}
				else
				{
					throw new Exception($"There is already a inactive org in the system with name {org.Name}, please reach out to admin to enable this org");
				}
			}
		}

		public async Task<List<Organization>> GetAllAsync()
		{
			return await this.SqlClient.GetAllData<Organization>();
		}

		public async Task<List<Organization>> GetAllForUserAsync(int userID)
		{
			string sql = $@"SELECT DISTINCT t.* FROM {TableName} (NOLOCK) t 
LEFT JOIN tblUserRoles r 
ON t.ID = r.OrganizationID OR r.OrganizationID IS NULL
where UserID = {userID}";
			return await this.SqlClient.GetData<Organization>(sql);
		}

		public async Task<Organization?> GetByNameAsync(string name)
		{
			string sql = $"SELECT * FROM {TableName} (NOLOCK) where Name = '{name}'";
			var results = await this.SqlClient.GetData<Organization>(sql);
			return results?.FirstOrDefault();
		}

		public async Task<Organization?> GetByIDAsync(int id)
		{
			string sql = $"SELECT * FROM {TableName} (NOLOCK) where ID = '{id}'";
			var results = await this.SqlClient.GetData<Organization>(sql);
			return results?.FirstOrDefault();
		}

		public async Task UpdateAsync(Organization org)
		{
			await this.SqlClient.UpdateAsync(org);
		}

		public async Task SetInactiveAsync(Organization org)
		{
			org.IsActive = false;
			await this.UpdateAsync(org);
		}

	}
}
