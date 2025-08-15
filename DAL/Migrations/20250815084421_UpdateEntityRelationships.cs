using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Installments_InstallmentId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Installments_InstallmentId",
                table: "Transactions",
                column: "InstallmentId",
                principalTable: "Installments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Installments_InstallmentId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Installments_InstallmentId",
                table: "Transactions",
                column: "InstallmentId",
                principalTable: "Installments",
                principalColumn: "Id");
        }
    }
}
