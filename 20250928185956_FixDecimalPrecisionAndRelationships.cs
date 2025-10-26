using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarEvents.Migrations
{
    /// <inheritdoc />
    public partial class FixDecimalPrecisionAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Venues_AspNetUsers_UserId",
                table: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_Venues_UserId",
                table: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.AlterColumn<decimal>(
                name: "TicketPrice",
                table: "Events",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "EventCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventCategories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Venues",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TicketPrice",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_UserId",
                table: "Venues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventCategories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "EventCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Venues_AspNetUsers_UserId",
                table: "Venues",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
