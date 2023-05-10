using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IOrganizationService
	{
		Task<Organization> AddAsync(Organization company);
		Task<List<Organization>> GetAllAsync();
		Task<List<Organization>> GetAllForUserAsync(int userID);
		Task<Organization?> GetByIDAsync(int id);
		Task<Organization?> GetByNameAsync(string name);
		Task SetInactiveAsync(Organization org);
		Task UpdateAsync(Organization company);
	}
}