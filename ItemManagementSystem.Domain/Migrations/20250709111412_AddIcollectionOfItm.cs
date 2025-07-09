using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemManagementSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddIcollectionOfItm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemRequestId",
                table: "RequestItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_ItemRequestId",
                table: "RequestItems",
                column: "ItemRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems",
                column: "ItemRequestId",
                principalTable: "ItemRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems");

            migrationBuilder.DropIndex(
                name: "IX_RequestItems_ItemRequestId",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "ItemRequestId",
                table: "RequestItems");
        }
    }
}
