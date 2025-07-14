using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Hotels",
                newName: "FullDescription");

            migrationBuilder.AddColumn<string>(
                name: "BriefDescription",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_ThumbnailId",
                table: "Hotels",
                column: "ThumbnailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Images_ThumbnailId",
                table: "Hotels",
                column: "ThumbnailId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Images_ThumbnailId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_ThumbnailId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "BriefDescription",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "FullDescription",
                table: "Hotels",
                newName: "Description");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
