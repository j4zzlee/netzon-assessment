using Microsoft.EntityFrameworkCore.Migrations;
using netzon_assetment.Utils;

namespace netzon_assetment.Migrations
{
    public partial class AddDefaultData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "roles",
            columns: new[] { "code", "label" },
            values: new object[] {"ADMIN", "Administrator" });

            migrationBuilder.InsertData(
            table: "users",
            columns: new[] { "firstname", "lastname", "email", "password" },
            values: new object[] { "Admin", "Admin", "admin@netzon.com", new PasswordHasher().HashPassword("n3tz0n@123") });

            migrationBuilder.InsertData(
            table: "users_roles",
            columns: new[] { "user_id", "role_id" },
            values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
