namespace Afrowave.SharedTools.Docs.Data
{
	/// <summary>
	/// Represents the database context for managing log entries in the application.
	/// </summary>
	/// <remarks>This context is used to interact with the "Logs" table in the database, which stores log entries.
	/// It provides access to the <see cref="Logs"/> DbSet for querying and saving log data.</remarks>
	/// <remarks>
	/// Initializes a new instance of the <see cref="DocsDbContext"/> class with the specified options.
	/// </remarks>
	/// <param name="options">The options to configure the <see cref="DocsDbContext"/>. This typically includes the database provider,
	/// connection string, and other settings required to initialize the context.</param>
	public class DocsDbContext(DbContextOptions<DocsDbContext> options) : DbContext(options)
	{
		/// <summary>
		/// Gets or sets the collection of administrators in the database.
		/// </summary>
		public DbSet<Admin> Admins { get; set; } = null!;

		/// <summary>
		/// Gets or sets the database table for managing documentation settings.
		/// </summary>
		public DbSet<ApplicationSettings> ApplicationSettings { get; set; } = null!;

		/// <summary>
		/// Gets or sets the collection of log entries in the database.
		/// </summary>
		public DbSet<LogEntry> Logs { get; set; } = null!;

		/// <summary>
		/// Gets or sets the collection of metadata folders in the database.
		/// </summary>
		public DbSet<MdFolder> MdFolders { get; set; } = null!;

		/// <summary>
		/// Configures the model for the database context by defining entity mappings and constraints.
		/// </summary>
		/// <remarks>This method is called by the Entity Framework runtime during model creation. It configures the
		/// <see cref="LogEntry"/> entity to map to the "Logs" table, sets up primary keys, and defines property constraints
		/// such as required fields, maximum lengths, and column types.</remarks>
		/// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure the entity mappings and relationships for the database context.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<LogEntry>(entity =>
			{
				entity.ToTable("Logs");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Timestamp)
					 .IsRequired();

				entity.Property(e => e.Level)
					 .HasMaxLength(10)
					 .IsRequired();

				entity.Property(e => e.Message)
					 .IsRequired();

				entity.Property(e => e.Exception)
					 .HasColumnType("TEXT");

				entity.Property(e => e.Properties)
					 .HasColumnType("TEXT");
			});
		}
	}
}