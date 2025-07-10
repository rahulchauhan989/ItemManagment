using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemManagementSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddItemModalToItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTypeId1",
                table: "ItemModels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_ItemTypeId1",
                table: "ItemModels",
                column: "ItemTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId1",
                table: "ItemModels",
                column: "ItemTypeId1",
                principalTable: "ItemTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId1",
                table: "ItemModels");

            migrationBuilder.DropIndex(
                name: "IX_ItemModels_ItemTypeId1",
                table: "ItemModels");

            migrationBuilder.DropColumn(
                name: "ItemTypeId1",
                table: "ItemModels");
        }
    }
}
