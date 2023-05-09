using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IOrganizationService<T>
	{
		Task<T> AddAsync(T company);
		Task<List<T>> GetAllAsync();
		Task<List<T>> GetAllForUserAsync(int userID);
		Task<T?> GetByIDAsync(int id);
		Task<T?> GetByNameAsync(string name);
		Task SetInactiveAsync(T org);
		Task UpdateAsync(T company);
	}
}