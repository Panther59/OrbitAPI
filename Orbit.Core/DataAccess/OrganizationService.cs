using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core.DataAccess
{
	public abstract class OrganizationService<T> : IOrganizationService<T>
		where T : Organization
	{
		private readonly ISqlClient sqlClient;
		private readonly string tableName;

		public ISqlClient SqlClient => sqlClient;

		public string TableName => tableName;

		public OrganizationService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
			var type = typeof(T);
			this.tableName = sqlClient.TableNameMapper(type);
			if (string.IsNullOrEmpty(this.tableName))
			{
				throw new Exception($"Unable to identify database table name for Type {type.FullName}");
			}
		}

		public async Task<T> AddAsync(T org)
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

		public async Task<List<T>> GetAllAsync()
		{
			return await this.SqlClient.GetAllData<T>();
		}

		public abstract Task<List<T>> GetAllForUserAsync(int userID);

		public async Task<T?> GetByNameAsync(string name)
		{
			string sql = $"SELECT * FROM {TableName} (NOLOCK) where Name = '{name}'";
			var results = await this.SqlClient.GetData<T>(sql);
			return results?.FirstOrDefault();
		}

		public async Task<T?> GetByIDAsync(int id)
		{
			string sql = $"SELECT * FROM {TableName} (NOLOCK) where ID = '{id}'";
			var results = await this.SqlClient.GetData<T>(sql);
			return results?.FirstOrDefault();
		}

		public async Task UpdateAsync(T org)
		{
			await this.SqlClient.UpdateAsync(org);
		}

		public async Task SetInactiveAsync(T org)
		{
			org.IsActive = false;
			await this.UpdateAsync(org);
		}

	}
}
