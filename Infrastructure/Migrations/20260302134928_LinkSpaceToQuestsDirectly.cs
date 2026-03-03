using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkSpaceToQuestsDirectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Groups_GroupId",
                table: "Quests");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Quests",
                newName: "SpaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Quests_GroupId",
                table: "Quests",
                newName: "IX_Quests_SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Spaces_SpaceId",
                table: "Quests",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Spaces_SpaceId",
                table: "Quests");

            migrationBuilder.RenameColumn(
                name: "SpaceId",
                table: "Quests",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Quests_SpaceId",
                table: "Quests",
                newName: "IX_Quests_GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Groups_GroupId",
                table: "Quests",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
