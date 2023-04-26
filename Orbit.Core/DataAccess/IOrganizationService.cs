using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IOrganizationService<T>
	{
		Task<T> AddAsync(T company);
		Task<List<T>> GetAllAsync();
		Task<T?> GetByIDAsync(int id);
		Task<T?> GetByNameAsync(string name);
		Task SetInactiveAsync(T org);
		Task UpdateAsync(T company);
	}
}