namespace Afrowave.SharedTools.Docs.Services
{
	public class AdminService(ILogger<AdminService> logger, DocsDbContext context)
	{
		/*
		 * Application admins are only registered users in the database. Who is authenticated is an admin. This service is used to manage admin-related operations.
		 * Only different is superuser that have more access to the system than a regular admin.
		 *
		 */
	}
}