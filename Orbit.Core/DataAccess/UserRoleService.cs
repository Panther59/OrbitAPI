using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class UserRoleService : IUserRoleService
	{
		private readonly ISqlClient sqlClient;

		public UserRoleService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
		}

		public async Task<List<UserRole>> GetUserRoles(int? userId = null, int? companyId = null, int? clientId = null)
		{
			string sql = $"EXEC [dbo].[spGetUserRoles] " +
				$"@userId = {(userId.HasValue ? userId.Value : "NULL")}, " +
				$"@companyId = {(companyId.HasValue ? companyId.Value : "NULL")}, " +
				$"@clientId = {(clientId.HasValue ? clientId.Value : "NULL")} ";

			return await this.sqlClient.GetData<UserRole>(sql);
		}

		public async Task<List<string>> GetUserPermissions(int userId, int? companyId = null, int? clientId = null)
		{
			string sql = $"EXEC [dbo].[spGetUserPermissions] " +
				$"@userId = {userId}, " +
				$"@companyId = {(companyId.HasValue ? companyId.Value : "NULL")}, " +
				$"@clientId = {(clientId.HasValue ? clientId.Value : "NULL")} ";

			return await this.sqlClient.GetData<string>(sql);
		}

		public async Task<List<Organization>> GetUserOrganizations(int userId, int? companyId = null, int? clientId = null)
		{
			var companyQuery = $"SELECT c.* FROM tblUserRoles (NOLOCK) ur INNER JOIN tblCompanies (NOLOCK) c ON ur.CompanyID = c.ID WHERE ur.UserID = {userId}";
			var clientQuery = $"SELECT c.* FROM tblUserRoles (NOLOCK) ur INNER JOIN tblClients (NOLOCK) c ON ur.ClientID = c.ID WHERE ur.UserID = {userId}";

			var companies = await this.sqlClient.GetData<Company>(companyQuery);
			var clients = await this.sqlClient.GetData<Client>(clientQuery);
			List<Organization> result = new List<Organization>();
			result.AddRange(companies);
			result.AddRange(clients);

			return result;
		}

		public async Task<bool> IsSuperUser(int userId)
		{
			var orgQuery = $"SELECT c.* FROM tblUserRoles (NOLOCK) ur WHERE ur.UserID = {userId} AND ur.ClientID is NULL and ur.CompanyID is NULL";
			var orgs = await this.sqlClient.GetData<Client>(orgQuery);
			return orgs.Any();
		}

		public async Task<List<Role>> GetAllRoles()
		{
			return await this.sqlClient.GetAllData<Role>();
		}

		public async Task<UserRole> AddRole(UserRole userRole)
		{
			var results = await this.GetUserRoles(userRole.UserID, userRole.CompanyID, userRole.ClientID);
			var existingUserRole = results?.FirstOrDefault();
			if (existingUserRole != null)
			{
				if (existingUserRole.RoleRanking == userRole.RoleRanking)
				{
					throw new Exception($"This permission already exists for {existingUserRole.User}");
				}
				else if (existingUserRole.RoleRanking < userRole.RoleRanking)
				{
					throw new Exception($"{existingUserRole.User} already has higher permission ({existingUserRole.Role}) available, if you want to change permission then edit existing permission");
				}
			}

			await this.sqlClient.InsertAsync(userRole);

			return userRole;
		}

		public async Task DeleteRole(UserRole userRole)
		{
			string sql = $"DELETE FROM tblUserRoles where UserID = '{userRole.UserID}' and RoleID = '{userRole.RoleID}'";
			if (userRole.CompanyID.HasValue)
			{
				sql += $" and CompanyID = '{userRole.CompanyID}'";
			}
			else if (userRole.ClientID.HasValue)
			{
				sql += $" and ClientID = '{userRole.ClientID}'";
			}

			await this.sqlClient.ExecuteAsync(sql);
		}


	}
}
