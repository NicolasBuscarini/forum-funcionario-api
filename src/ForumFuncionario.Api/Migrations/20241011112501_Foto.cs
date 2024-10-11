using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumFuncionario.Api.Migrations
{
    /// <inheritdoc />
    public partial class Foto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "AspNetUsers");
        }
    }
}
