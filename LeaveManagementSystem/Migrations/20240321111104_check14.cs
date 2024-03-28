using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class check14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDetails_gender_EmployeeGenderId",
                table: "EmployeeDetails");

            migrationBuilder.DropTable(
                name: "gender");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDetails_EmployeeGenderId",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "EmployeeGenderId",
                table: "EmployeeDetails");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeGender",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeGender",
                table: "EmployeeDetails");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeGenderId",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "gender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    gender = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gender", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_EmployeeGenderId",
                table: "EmployeeDetails",
                column: "EmployeeGenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDetails_gender_EmployeeGenderId",
                table: "EmployeeDetails",
                column: "EmployeeGenderId",
                principalTable: "gender",
                principalColumn: "Id");
        }
    }
}
