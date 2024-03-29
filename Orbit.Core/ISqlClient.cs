﻿namespace Orbit.Core
{
	public interface ISqlClient
	{
		Task<int> ExecuteAsync(string sql);
		Task<List<T>> GetAllData<T>() where T : class;
		Task<List<T>> GetData<T>(string query) where T : class;
		Task<int?> GetMaxID(string table);
		Task InsertAsync<T>(IEnumerable<T> data) where T : class;
		Task InsertAsync<T>(T data) where T : class;
		Task UpdateAsync<T>(IEnumerable<T> data) where T : class;
		Task UpdateAsync<T>(T data) where T : class;
		string TableNameMapper(Type type);
	}
}