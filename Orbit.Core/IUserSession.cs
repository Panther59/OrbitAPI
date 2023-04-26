namespace Orbit.Core
{
	public interface IUserSession
	{
		int? UserID { get; }
		string? Picture { get; }
	}
}