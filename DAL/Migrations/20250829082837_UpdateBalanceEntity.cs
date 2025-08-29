using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBalanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AsOf",
                table: "Balances",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Balances",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Balances");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Balances",
                newName: "AsOf");
        }
    }
}
