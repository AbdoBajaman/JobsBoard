using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobsBoard.Migrations
{
    public partial class Add_Benifts_and_QualificationsANDExperiences_Columns_InJobModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QualificationsANDExperiences",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "QualificationsANDExperiences",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Job");
        }
    }
}
