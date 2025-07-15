using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItemManagementSystem.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfTableAndCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId1",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_request_item_details_RequestItemId",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_users_CreatedBy",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemModels_users_ModifiedBy",
                table: "ItemModels");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTypes_users_CreatedBy",
                table: "ItemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTypes_users_ModifiedBy",
                table: "ItemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_request_item_details_ItemModels_item_model_id",
                table: "purchase_request_item_details");

            migrationBuilder.DropForeignKey(
                name: "FK_request_item_details_ItemModels_item_model_id",
                table: "request_item_details");

            migrationBuilder.DropForeignKey(
                name: "FK_return_request_item_details_ItemModels_item_model_id",
                table: "return_request_item_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTypes",
                table: "ItemTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemModels",
                table: "ItemModels");

            migrationBuilder.RenameTable(
                name: "ItemTypes",
                newName: "item_types");

            migrationBuilder.RenameTable(
                name: "ItemModels",
                newName: "item_models");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "item_types",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "item_types",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "item_types",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "item_types",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "item_types",
                newName: "modified_by");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "item_types",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "item_types",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "item_types",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTypes_ModifiedBy",
                table: "item_types",
                newName: "IX_item_types_modified_by");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTypes_CreatedBy",
                table: "item_types",
                newName: "IX_item_types_created_by");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "item_models",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "item_models",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "item_models",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "item_models",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "item_models",
                newName: "modified_by");

            migrationBuilder.RenameColumn(
                name: "ItemTypeId",
                table: "item_models",
                newName: "item_type_id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "item_models",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "item_models",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "item_models",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_ItemModels_RequestItemId",
                table: "item_models",
                newName: "IX_item_models_RequestItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemModels_ModifiedBy",
                table: "item_models",
                newName: "IX_item_models_modified_by");

            migrationBuilder.RenameIndex(
                name: "IX_ItemModels_ItemTypeId1",
                table: "item_models",
                newName: "IX_item_models_ItemTypeId1");

            migrationBuilder.RenameIndex(
                name: "IX_ItemModels_ItemTypeId",
                table: "item_models",
                newName: "IX_item_models_item_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_ItemModels_CreatedBy",
                table: "item_models",
                newName: "IX_item_models_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_types",
                table: "item_types",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_item_models",
                table: "item_models",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_item_models_item_types_ItemTypeId1",
                table: "item_models",
                column: "ItemTypeId1",
                principalTable: "item_types",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_item_models_item_types_item_type_id",
                table: "item_models",
                column: "item_type_id",
                principalTable: "item_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_item_models_request_item_details_RequestItemId",
                table: "item_models",
                column: "RequestItemId",
                principalTable: "request_item_details",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_item_models_users_created_by",
                table: "item_models",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_item_models_users_modified_by",
                table: "item_models",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_item_types_users_created_by",
                table: "item_types",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_item_types_users_modified_by",
                table: "item_types",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_request_item_details_item_models_item_model_id",
                table: "purchase_request_item_details",
                column: "item_model_id",
                principalTable: "item_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_request_item_details_item_models_item_model_id",
                table: "request_item_details",
                column: "item_model_id",
                principalTable: "item_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_return_request_item_details_item_models_item_model_id",
                table: "return_request_item_details",
                column: "item_model_id",
                principalTable: "item_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_item_models_item_types_ItemTypeId1",
                table: "item_models");

            migrationBuilder.DropForeignKey(
                name: "FK_item_models_item_types_item_type_id",
                table: "item_models");

            migrationBuilder.DropForeignKey(
                name: "FK_item_models_request_item_details_RequestItemId",
                table: "item_models");

            migrationBuilder.DropForeignKey(
                name: "FK_item_models_users_created_by",
                table: "item_models");

            migrationBuilder.DropForeignKey(
                name: "FK_item_models_users_modified_by",
                table: "item_models");

            migrationBuilder.DropForeignKey(
                name: "FK_item_types_users_created_by",
                table: "item_types");

            migrationBuilder.DropForeignKey(
                name: "FK_item_types_users_modified_by",
                table: "item_types");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_request_item_details_item_models_item_model_id",
                table: "purchase_request_item_details");

            migrationBuilder.DropForeignKey(
                name: "FK_request_item_details_item_models_item_model_id",
                table: "request_item_details");

            migrationBuilder.DropForeignKey(
                name: "FK_return_request_item_details_item_models_item_model_id",
                table: "return_request_item_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_item_types",
                table: "item_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_item_models",
                table: "item_models");

            migrationBuilder.RenameTable(
                name: "item_types",
                newName: "ItemTypes");

            migrationBuilder.RenameTable(
                name: "item_models",
                newName: "ItemModels");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ItemTypes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "ItemTypes",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ItemTypes",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "modified_by",
                table: "ItemTypes",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ItemTypes",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ItemTypes",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ItemTypes",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_item_types_modified_by",
                table: "ItemTypes",
                newName: "IX_ItemTypes_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_item_types_created_by",
                table: "ItemTypes",
                newName: "IX_ItemTypes_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ItemModels",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "ItemModels",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ItemModels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ItemModels",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "modified_by",
                table: "ItemModels",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "item_type_id",
                table: "ItemModels",
                newName: "ItemTypeId");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ItemModels",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ItemModels",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ItemModels",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_item_models_RequestItemId",
                table: "ItemModels",
                newName: "IX_ItemModels_RequestItemId");

            migrationBuilder.RenameIndex(
                name: "IX_item_models_modified_by",
                table: "ItemModels",
                newName: "IX_ItemModels_ModifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_item_models_ItemTypeId1",
                table: "ItemModels",
                newName: "IX_ItemModels_ItemTypeId1");

            migrationBuilder.RenameIndex(
                name: "IX_item_models_item_type_id",
                table: "ItemModels",
                newName: "IX_ItemModels_ItemTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_item_models_created_by",
                table: "ItemModels",
                newName: "IX_ItemModels_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTypes",
                table: "ItemTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemModels",
                table: "ItemModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId",
                table: "ItemModels",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_ItemTypes_ItemTypeId1",
                table: "ItemModels",
                column: "ItemTypeId1",
                principalTable: "ItemTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_request_item_details_RequestItemId",
                table: "ItemModels",
                column: "RequestItemId",
                principalTable: "request_item_details",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_users_CreatedBy",
                table: "ItemModels",
                column: "CreatedBy",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemModels_users_ModifiedBy",
                table: "ItemModels",
                column: "ModifiedBy",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTypes_users_CreatedBy",
                table: "ItemTypes",
                column: "CreatedBy",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTypes_users_ModifiedBy",
                table: "ItemTypes",
                column: "ModifiedBy",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_request_item_details_ItemModels_item_model_id",
                table: "purchase_request_item_details",
                column: "item_model_id",
                principalTable: "ItemModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_request_item_details_ItemModels_item_model_id",
                table: "request_item_details",
                column: "item_model_id",
                principalTable: "ItemModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_return_request_item_details_ItemModels_item_model_id",
                table: "return_request_item_details",
                column: "item_model_id",
                principalTable: "ItemModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
