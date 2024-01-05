using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalStore.Migrations
{
    /// <inheritdoc />
    public partial class Addrentalplanetc1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_rentalPlans_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                newName: "RentalPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_RentalPlanId");

            migrationBuilder.AlterColumn<int>(
                name: "RentalPlanId",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_rentalPlans_RentalPlanId",
                schema: "Identity",
                table: "AspNetUsers",
                column: "RentalPlanId",
                principalSchema: "Identity",
                principalTable: "rentalPlans",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_rentalPlans_RentalPlanId",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RentalPlanId",
                schema: "Identity",
                table: "AspNetUsers",
                newName: "RentalPlanid");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_RentalPlanId",
                schema: "Identity",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_RentalPlanid");

            migrationBuilder.AlterColumn<int>(
                name: "RentalPlanid",
                schema: "Identity",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
