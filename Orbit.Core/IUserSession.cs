using Orbit.Models.OrbitDB;

namespace Orbit.Core
{
	public interface IUserSession
	{
		int? UserID { get; }
		string? Picture { get; }
		List<UserRole> Roles { get; }
		bool HasPermission(string role, int? companyId = null);
		void SetUpdatedAuditColumns(BaseAuditRecord record);
		void SetCreatedAuditColumns(BaseAuditRecord record, BaseAuditRecord? originalRecord = null);
	}
}