using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Auth_Service.Migrations
{
    /// <inheritdoc />
    public partial class historyconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistories_TaskId",
                table: "TaskStatusHistories",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistories_ProjectTasks_TaskId",
                table: "TaskStatusHistories",
                column: "TaskId",
                principalTable: "ProjectTasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistories_ProjectTasks_TaskId",
                table: "TaskStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskStatusHistories_TaskId",
                table: "TaskStatusHistories");
        }
    }
}
