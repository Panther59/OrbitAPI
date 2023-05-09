using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class ClientService : OrganizationService<Client>, IClientService
	{
		public ClientService(ISqlClient sqlClient) : base(sqlClient)
		{
		}

		public async override Task<List<Client>> GetAllForUserAsync(int userID)
		{
			string sql = $@"SELECT DISTINCT t.* FROM {this.TableName} (NOLOCK) t 
LEFT JOIN tblUserRoles r 
ON t.ID = r.ClientID OR r.CompanyID IS NULL and r.ClientID IS NULL
where UserID = {userID}";
			return await this.SqlClient.GetData<Client>(sql);
		}
	}
}
