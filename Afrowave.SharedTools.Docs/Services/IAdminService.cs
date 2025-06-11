
namespace Afrowave.SharedTools.Docs.Services
{
	public interface IAdminService
	{
		Task<bool> CanEmailRegister(string email);
		Task<Response<string>> ChangeAdminEmail(string oldEmail, string newEmail);
		Task<Response<bool>> ConfirmEmail(string email);
		Task<Response<bool>> DeleteAdminByEmailAsync(string email);
		Task<Response<Admin>> GetAdminByEmailAsync(string email);
		Task<Response<string>> GetBearer(string email);
		Task<Response<Admin>> RegisterAdminAsync(RegisterAdminModel model);
		Task<Response<bool>> SetBearer(string email, string bearer);
	}
}