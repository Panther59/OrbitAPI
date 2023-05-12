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
		void SetUpdatedAuditColumns(BaseAuditRecord record, DateTime? dateTime = null);
		void SetCreatedAuditColumns(BaseAuditRecord record, BaseAuditRecord ? originalRecord = null, DateTime? dateTime = null);
	}
}