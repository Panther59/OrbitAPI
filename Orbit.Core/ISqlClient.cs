using Microsoft.Data.SqlClient;

namespace Orbit.Core
{
	public interface ISqlClient
	{
		Task<int> ExecuteAsync(string sql);
		Task<List<T>> GetAllData<T>() where T : class;
		Task<List<T>> GetData<T>(string query) where T : class;
		Task<T> GetDataByID<T>(int id) where T : class;
		Task<int?> GetMaxID<T>() where T : class;
		Task<int?> GetMaxID(string table);
		Task DeleteAsync<T>(T data) where T : class;
		Task DeleteAsync<T>(int id) where T : class;
		Task InsertAsync<T>(IEnumerable<T> data) where T : class;
		Task InsertAsync<T>(T data) where T : class;
		Task UpdateAsync<T>(IEnumerable<T> data) where T : class;
		Task UpdateAsync<T>(T data) where T : class;
		string TableNameMapper(Type type);
		SqlTransaction GetTransaction();
	}
}