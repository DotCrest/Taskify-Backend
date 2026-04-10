using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExplictlyAddUserQuestsTableAndUpdatedAtColumnToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuest_UsersAccount_UserId",
                table: "UserQuest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuest",
                table: "UserQuest");

            migrationBuilder.RenameTable(
                name: "UserQuest",
                newName: "UserQuests");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuest_QuestId",
                table: "UserQuests",
                newName: "IX_UserQuests_QuestId");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuests",
                table: "UserQuests",
                columns: new[] { "UserId", "QuestId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuests_Quests_QuestId",
                table: "UserQuests",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuests_UsersAccount_UserId",
                table: "UserQuests",
                column: "UserId",
                principalTable: "UsersAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuests_Quests_QuestId",
                table: "UserQuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuests_UsersAccount_UserId",
                table: "UserQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuests",
                table: "UserQuests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "UserQuests",
                newName: "UserQuest");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuests_QuestId",
                table: "UserQuest",
                newName: "IX_UserQuest_QuestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuest",
                table: "UserQuest",
                columns: new[] { "UserId", "QuestId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuest_UsersAccount_UserId",
                table: "UserQuest",
                column: "UserId",
                principalTable: "UsersAccount",
                principalColumn: "Id");
        }
    }
}
