using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrowave.SharedTools.Docs.Migrations
{
	/// <inheritdoc />
	public partial class fix : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				 name: "Host",
				 table: "ApplicationSettings",
				 newName: "SmtpHost");

			migrationBuilder.RenameColumn(
				 name: "EncryptedPasswoord",
				 table: "ApplicationSettings",
				 newName: "EncryptedPassword");

			migrationBuilder.AddColumn<string>(
				 name: "ApiKey",
				 table: "ApplicationSettings",
				 type: "TEXT",
				 nullable: false,
				 defaultValue: "");

			migrationBuilder.AddColumn<bool>(
				 name: "IsActive",
				 table: "ApplicationSettings",
				 type: "INTEGER",
				 nullable: false,
				 defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				 name: "SuccessfullyTested",
				 table: "ApplicationSettings",
				 type: "INTEGER",
				 nullable: false,
				 defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				 name: "IsEmailConfirmed",
				 table: "Admins",
				 type: "INTEGER",
				 nullable: false,
				 defaultValue: false);

			migrationBuilder.AddColumn<DateTime>(
				 name: "LastSeen",
				 table: "Admins",
				 type: "TEXT",
				 nullable: false,
				 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
				 name: "RegistrationDate",
				 table: "Admins",
				 type: "TEXT",
				 nullable: false,
				 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<int>(
				 name: "Role",
				 table: "Admins",
				 type: "INTEGER",
				 nullable: false,
				 defaultValue: 0);

			migrationBuilder.AddColumn<TimeSpan>(
				 name: "TimeOnline",
				 table: "Admins",
				 type: "TEXT",
				 nullable: false,
				 defaultValue: new TimeSpan(0, 0, 0, 0, 0));

			migrationBuilder.CreateTable(
				 name: "MdFolders",
				 columns: table => new
				 {
					 Id = table.Column<int>(type: "INTEGER", nullable: false)
							.Annotation("Sqlite:Autoincrement", true),
					 Path = table.Column<string>(type: "TEXT", nullable: false),
					 Name = table.Column<string>(type: "TEXT", nullable: false),
					 LastText = table.Column<string>(type: "TEXT", nullable: false),
					 LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
				 },
				 constraints: table =>
				 {
					 table.PrimaryKey("PK_MdFolders", x => x.Id);
				 });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				 name: "MdFolders");

			migrationBuilder.DropColumn(
				 name: "ApiKey",
				 table: "ApplicationSettings");

			migrationBuilder.DropColumn(
				 name: "IsActive",
				 table: "ApplicationSettings");

			migrationBuilder.DropColumn(
				 name: "SuccessfullyTested",
				 table: "ApplicationSettings");

			migrationBuilder.DropColumn(
				 name: "IsEmailConfirmed",
				 table: "Admins");

			migrationBuilder.DropColumn(
				 name: "LastSeen",
				 table: "Admins");

			migrationBuilder.DropColumn(
				 name: "RegistrationDate",
				 table: "Admins");

			migrationBuilder.DropColumn(
				 name: "Role",
				 table: "Admins");

			migrationBuilder.DropColumn(
				 name: "TimeOnline",
				 table: "Admins");

			migrationBuilder.RenameColumn(
				 name: "SmtpHost",
				 table: "ApplicationSettings",
				 newName: "Host");

			migrationBuilder.RenameColumn(
				 name: "EncryptedPassword",
				 table: "ApplicationSettings",
				 newName: "EncryptedPasswoord");
		}
	}
}