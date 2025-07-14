using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemManagementSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_ItemRequests_RequestId",
                table: "RequestItems");

            migrationBuilder.DropIndex(
                name: "IX_RequestItems_RequestId",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "RequestItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemRequestId",
                table: "RequestItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemRequestId1",
                table: "RequestItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestItemId",
                table: "ItemModels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_ItemRequestId1",
                table: "RequestItems",
                column: "ItemRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_RequestItemId",
                table: "ItemModels",
                column: "RequestItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_RequestItems_RequestItemId",
                table: "ItemModels",
                column: "RequestItemId",
                principalTable: "RequestItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems",
                column: "ItemRequestId",
                principalTable: "ItemRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId1",
                table: "RequestItems",
                column: "ItemRequestId1",
                principalTable: "ItemRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_RequestItems_RequestItemId",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId1",
                table: "RequestItems");

            migrationBuilder.DropIndex(
                name: "IX_RequestItems_ItemRequestId1",
                table: "RequestItems");

            migrationBuilder.DropIndex(
                name: "IX_ItemModels_RequestItemId",
                table: "ItemModels");

            migrationBuilder.DropColumn(
                name: "ItemRequestId1",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "RequestItemId",
                table: "ItemModels");

            migrationBuilder.AlterColumn<int>(
                name: "ItemRequestId",
                table: "RequestItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "RequestItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_RequestId",
                table: "RequestItems",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_ItemRequests_ItemRequestId",
                table: "RequestItems",
                column: "ItemRequestId",
                principalTable: "ItemRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_ItemRequests_RequestId",
                table: "RequestItems",
                column: "RequestId",
                principalTable: "ItemRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
