using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class CompanyService : OrganizationService<Company>, ICompanyService
	{
		public CompanyService(ISqlClient sqlClient) : base(sqlClient)
		{
		}

		public async override Task<List<Company>> GetAllForUserAsync(int userID)
		{
			string sql = $@"SELECT DISTINCT t.* FROM {this.TableName} (NOLOCK) t 
LEFT JOIN tblUserRoles r 
ON t.ID = r.CompanyID OR r.CompanyID IS NULL and r.ClientID IS NULL
where UserID = {userID}";
			return await this.SqlClient.GetData<Company>(sql);
		}
	}
}
