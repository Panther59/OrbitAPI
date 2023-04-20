using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Orbit.Models.OrbitDB;
using Orbit.Models.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core
{
	public class SqlClient : ISqlClient
	{
		private readonly ConnectionStrings connectionStrings;

		public SqlClient(IOptions<ConnectionStrings> connectionStrings)
		{
			this.connectionStrings = connectionStrings.Value;
		}

		public async Task<List<T>> GetData<T>(string query) where T : class
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				var result = await connection.QueryAsync<T>(query);
				return result.ToList();
			}
		}

		public async Task<int?> GetMaxID(string table)
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				var result = await connection.QueryFirstOrDefaultAsync<User>($"SELECT MAX(ID) as ID FROM {table} (NOLOCK)");
				return result?.ID;
			}
		}

		public async Task InsertAsync<T>(T data) where T : class
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				await connection.InsertAsync<T>(data);
			}
		}

		public async Task InsertAsync<T>(IEnumerable<T> data) where T : class
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				using (SqlTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						foreach (var item in data)
						{
							await connection.InsertAsync<T>(item, transaction);
						}

						transaction.Commit();
					}
					catch
					{
						transaction.Rollback();
						throw;
					}

				}
			}
		}

		public async Task UpdateAsync<T>(T data) where T : class
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				await connection.UpdateAsync<T>(data);
			}
		}

		public async Task UpdateAsync<T>(IEnumerable<T> data) where T : class
		{
			using (SqlConnection connection = new SqlConnection(this.connectionStrings.OrbitDB))
			{
				using (SqlTransaction transaction = connection.BeginTransaction())
				{
					try
					{
						foreach (var item in data)
						{
							await connection.UpdateAsync<T>(item, transaction);
						}

						transaction.Commit();
					}
					catch
					{
						transaction.Rollback();
						throw;
					}

				}
			}
		}

	}
}
