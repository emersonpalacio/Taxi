using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class inic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupDetails_UserGroupDetails_UserGroupDetailEntityId",
                table: "UserGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupDetails_UserGroups_UserGroupEntityId",
                table: "UserGroupDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserGroupDetails_UserGroupDetailEntityId",
                table: "UserGroupDetails");

            migrationBuilder.DropColumn(
                name: "UserGroupDetailEntityId",
                table: "UserGroupDetails");

            migrationBuilder.RenameColumn(
                name: "UserGroupEntityId",
                table: "UserGroupDetails",
                newName: "UserGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupDetails_UserGroupEntityId",
                table: "UserGroupDetails",
                newName: "IX_UserGroupDetails_UserGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupDetails_UserGroups_UserGroupId",
                table: "UserGroupDetails",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroupDetails_UserGroups_UserGroupId",
                table: "UserGroupDetails");

            migrationBuilder.RenameColumn(
                name: "UserGroupId",
                table: "UserGroupDetails",
                newName: "UserGroupEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroupDetails_UserGroupId",
                table: "UserGroupDetails",
                newName: "IX_UserGroupDetails_UserGroupEntityId");

            migrationBuilder.AddColumn<int>(
                name: "UserGroupDetailEntityId",
                table: "UserGroupDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupDetails_UserGroupDetailEntityId",
                table: "UserGroupDetails",
                column: "UserGroupDetailEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupDetails_UserGroupDetails_UserGroupDetailEntityId",
                table: "UserGroupDetails",
                column: "UserGroupDetailEntityId",
                principalTable: "UserGroupDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroupDetails_UserGroups_UserGroupEntityId",
                table: "UserGroupDetails",
                column: "UserGroupEntityId",
                principalTable: "UserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
