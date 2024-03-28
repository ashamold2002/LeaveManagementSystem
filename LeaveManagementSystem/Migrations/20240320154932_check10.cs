using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class check10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leaveAllocateds_onDutyDetails_OnDutyDetailsId",
                table: "leaveAllocateds");

            migrationBuilder.DropTable(
                name: "onDutyDetails");

            migrationBuilder.DropIndex(
                name: "IX_leaveAllocateds_OnDutyDetailsId",
                table: "leaveAllocateds");

            migrationBuilder.RenameColumn(
                name: "OnDutyDetailsId",
                table: "leaveAllocateds",
                newName: "OnDuty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnDuty",
                table: "leaveAllocateds",
                newName: "OnDutyDetailsId");

            migrationBuilder.CreateTable(
                name: "onDutyDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onDutyDetails", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_leaveAllocateds_OnDutyDetailsId",
                table: "leaveAllocateds",
                column: "OnDutyDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_leaveAllocateds_onDutyDetails_OnDutyDetailsId",
                table: "leaveAllocateds",
                column: "OnDutyDetailsId",
                principalTable: "onDutyDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
