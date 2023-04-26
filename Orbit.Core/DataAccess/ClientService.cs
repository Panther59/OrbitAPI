using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class ClientService : OrganizationService<Client>, IClientService
	{
		public ClientService(ISqlClient sqlClient) : base(sqlClient)
		{
		}
	}
}
