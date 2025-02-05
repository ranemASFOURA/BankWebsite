using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking_Website.Migrations
{
    public partial class updatetrnsactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sourceAccountNumber",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "targetAccountNumber",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sourceAccountNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "targetAccountNumber",
                table: "Transactions");
        }
    }
}
