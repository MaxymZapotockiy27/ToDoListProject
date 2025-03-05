using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListProj.Migrations
{
    /// <inheritdoc />
    public partial class AddingImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupImageUrl",
                table: "GroupTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupImageUrl",
                table: "GroupTasks");
        }
    }
}
