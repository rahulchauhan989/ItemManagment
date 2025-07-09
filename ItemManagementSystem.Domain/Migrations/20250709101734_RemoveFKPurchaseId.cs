using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemManagementSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFKPurchaseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemRequests_PurchaseRequests_PurchaseId",
                table: "ItemRequests");

            migrationBuilder.DropIndex(
                name: "IX_ItemRequests_PurchaseId",
                table: "ItemRequests");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "ItemRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseId",
                table: "ItemRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequests_PurchaseId",
                table: "ItemRequests",
                column: "PurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRequests_PurchaseRequests_PurchaseId",
                table: "ItemRequests",
                column: "PurchaseId",
                principalTable: "PurchaseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
