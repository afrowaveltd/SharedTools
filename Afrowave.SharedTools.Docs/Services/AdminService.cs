namespace Afrowave.SharedTools.Docs.Services
{
	public class AdminService(ILogger<AdminService> logger,
		DocsDbContext context,
		IStringLocalizer<AdminService> localizer)
	{
		private readonly ILogger<AdminService> _logger = logger;
		private readonly DocsDbContext _context = context;
		private readonly IStringLocalizer<AdminService> _localizer = localizer;

		public async Task<Response<Admin>> RegisterAdminAsync(RegisterAdminModel model)
		{
			if(model == null)
			{
				_logger.LogError("RegisterAdminAsync: Model is null.");
				return Response<Admin>.Fail(_localizer["Invalid model."]);
			}
			var existingAdmin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == model.Email);
			if(existingAdmin != null)
			{
				_logger.LogWarning("RegisterAdminAsync: Admin with email {Email} already exists.", model.Email);
				return Response<Admin>.Fail(_localizer["Email already registered."]);
			}
			if(string.IsNullOrWhiteSpace(model.DisplayName))
			{
				model.DisplayName = model.Email; // Default to email if display name is not provided
			}
			var newAdmin = new Admin
			{
				Email = model.Email,
				DisplayName = model.DisplayName,
				IsActive = true
			};
			await _context.Admins.AddAsync(newAdmin);
			await _context.SaveChangesAsync();
			_logger.LogInformation("RegisterAdminAsync: New admin registered with email {Email}.", model.Email);
			return Response<Admin>.Successful(newAdmin, _localizer["Registration successful."]);
		}

		public async Task<Response<Admin>> GetAdminByEmailAsync(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				_logger.LogError("GetAdminByEmailAsync: Email is null or empty.");
				return Response<Admin>.Fail(_localizer["Email cannot be null or empty."]);
			}
			var admin = await _context.Admins
				.FirstOrDefaultAsync(a => a.Email == email);
			if(admin == null)
			{
				_logger.LogWarning("GetAdminByEmailAsync: No admin found with email {Email}.", email);
				return Response<Admin>.Fail(_localizer["Admin not found."]);
			}
			return Response<Admin>.Successful(admin, _localizer["Admin retrieved successfully."]);
		}

		public async Task<bool> CanEmailRegister(string email)
		{
			email = email.ToLowerInvariant().Trim();
			return !await _context.Admins.AnyAsync(s => s.Email == email);
		}
	}
}