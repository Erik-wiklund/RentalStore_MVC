using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalStore.Migrations
{
    /// <inheritdoc />
    public partial class Addrentalplanetc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "rentalPlans",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RentalPlanName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentalPlans", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                column: "RentalPlanid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_rentalPlans_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                column: "RentalPlanid",
                principalSchema: "Identity",
                principalTable: "rentalPlans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_rentalPlans_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "rentalPlans",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
