using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListProj.Migrations
{
    /// <inheritdoc />
    public partial class GroupTaskAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_GroupTasks_GroupId",
                table: "GroupMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupMembers");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupMembers",
                newName: "GroupTaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                columns: new[] { "GroupTaskId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_GroupTasks_GroupTaskId",
                table: "GroupMembers",
                column: "GroupTaskId",
                principalTable: "GroupTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_GroupTasks_GroupTaskId",
                table: "GroupMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.RenameColumn(
                name: "GroupTaskId",
                table: "GroupMembers",
                newName: "GroupId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GroupMembers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_GroupTasks_GroupId",
                table: "GroupMembers",
                column: "GroupId",
                principalTable: "GroupTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
