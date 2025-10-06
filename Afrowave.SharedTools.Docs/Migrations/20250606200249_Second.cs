using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrowave.SharedTools.Docs.Migrations
{
	/// <inheritdoc />
	public partial class Second : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				 name: "Admins",
				 columns: table => new
				 {
					 Id = table.Column<int>(type: "INTEGER", nullable: false)
							.Annotation("Sqlite:Autoincrement", true),
					 Otp = table.Column<string>(type: "TEXT", nullable: false),
					 OtpValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
					 Email = table.Column<string>(type: "TEXT", nullable: false),
					 IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
					 Bearer = table.Column<string>(type: "TEXT", nullable: false)
				 },
				 constraints: table =>
				 {
					 table.PrimaryKey("PK_Admins", x => x.Id);
				 });

			migrationBuilder.CreateTable(
				 name: "DocsSettings",
				 columns: table => new
				 {
					 Name = table.Column<string>(type: "TEXT", nullable: false),
					 MdAbout = table.Column<string>(type: "TEXT", nullable: true),
					 MdAboutLink = table.Column<string>(type: "TEXT", nullable: true)
				 },
				 constraints: table =>
				 {
					 table.PrimaryKey("PK_DocsSettings", x => x.Name);
				 });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				 name: "Admins");

			migrationBuilder.DropTable(
				 name: "DocsSettings");
		}
	}
}