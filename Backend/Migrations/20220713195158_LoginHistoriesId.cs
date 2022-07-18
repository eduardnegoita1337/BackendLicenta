using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class LoginHistoriesId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LoginHistories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginHistories",
                table: "LoginHistories",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginHistories",
                table: "LoginHistories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LoginHistories");
        }
    }
}
