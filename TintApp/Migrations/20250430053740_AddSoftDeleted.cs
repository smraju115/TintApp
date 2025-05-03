using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TintApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceCategories");
        }
    }
}
