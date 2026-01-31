using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeleteTechnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Quests_QuestId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_UsersAccount_SenderId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Workspaces_WorkspaceId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_UsersAccount_AuthorId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceMembers_Spaces_SpaceId",
                table: "SpaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_UsersAccount_UserId",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_UsersAccount_OwnerId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMember_User_Workspace_Active",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "UsersAccount");

            migrationBuilder.DropIndex(
                name: "IX_QuestUser_Active_Quest",
                table: "UserQuest");

            migrationBuilder.DropIndex(
                name: "IX_QuestUser_Active_User",
                table: "UserQuest");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkspaceMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkspaceMembers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UsersAccount");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UsersAccount");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserQuest");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserQuest");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SpaceMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SpaceMembers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Quests",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_UserId",
                table: "WorkspaceMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "UsersAccount",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuest_QuestId",
                table: "UserQuest",
                column: "QuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Quests_QuestId",
                table: "Comments",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_UsersAccount_SenderId",
                table: "Invitations",
                column: "SenderId",
                principalTable: "UsersAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Workspaces_WorkspaceId",
                table: "Invitations",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_UsersAccount_AuthorId",
                table: "Quests",
                column: "AuthorId",
                principalTable: "UsersAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceMembers_Spaces_SpaceId",
                table: "SpaceMembers",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_UsersAccount_UserId",
                table: "WorkspaceMembers",
                column: "UserId",
                principalTable: "UsersAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_UsersAccount_OwnerId",
                table: "Workspaces",
                column: "OwnerId",
                principalTable: "UsersAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Quests_QuestId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_UsersAccount_SenderId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Workspaces_WorkspaceId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_UsersAccount_AuthorId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceMembers_Spaces_SpaceId",
                table: "SpaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_UsersAccount_UserId",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_UsersAccount_OwnerId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_UserId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "UsersAccount");

            migrationBuilder.DropIndex(
                name: "IX_UserQuest_QuestId",
                table: "UserQuest");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Workspaces",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Workspaces",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkspaceMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkspaceMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UsersAccount",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UsersAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserQuest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserQuest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Tags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Spaces",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Spaces",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SpaceMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SpaceMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Quests",
                type: "nvarchar(128)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Quests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Quests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Invitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Groups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Comments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMember_User_Workspace_Active",
                table: "WorkspaceMembers",
                columns: new[] { "UserId", "WorkspaceId" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "UsersAccount",
                column: "NormalizedEmail",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestUser_Active_Quest",
                table: "UserQuest",
                columns: new[] { "QuestId", "IsDeleted" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestUser_Active_User",
                table: "UserQuest",
                columns: new[] { "UserId", "IsDeleted" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Quests_QuestId",
                table: "Comments",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_UsersAccount_SenderId",
                table: "Invitations",
                column: "SenderId",
                principalTable: "UsersAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Workspaces_WorkspaceId",
                table: "Invitations",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_UsersAccount_AuthorId",
                table: "Quests",
                column: "AuthorId",
                principalTable: "UsersAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceMembers_Spaces_SpaceId",
                table: "SpaceMembers",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuest_Quests_QuestId",
                table: "UserQuest",
                column: "QuestId",
                principalTable: "Quests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_UsersAccount_UserId",
                table: "WorkspaceMembers",
                column: "UserId",
                principalTable: "UsersAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                table: "WorkspaceMembers",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_UsersAccount_OwnerId",
                table: "Workspaces",
                column: "OwnerId",
                principalTable: "UsersAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
