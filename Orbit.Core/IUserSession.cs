using Orbit.Models.OrbitDB;

namespace Orbit.Core
{
	public interface IUserSession
	{
		int? UserID { get; }
		Organization? Organization { get; }
		string? Picture { get; }
		List<string>? Permissions { get; }
		bool HasPermission(string role);
		void SetUpdatedAuditColumns(BaseAuditRecord record);
		void SetCreatedAuditColumns(BaseAuditRecord record, BaseAuditRecord? originalRecord = null);
	}
}