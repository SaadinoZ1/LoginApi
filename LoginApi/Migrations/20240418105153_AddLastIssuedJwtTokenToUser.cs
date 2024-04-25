using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLastIssuedJwtTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastIssuedJwtToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastIssuedJwtToken",
                table: "User");
        }
    }
}
