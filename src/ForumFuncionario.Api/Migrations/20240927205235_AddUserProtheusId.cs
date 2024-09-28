using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumFuncionario.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProtheusId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RaMatricula",
                table: "AspNetUsers",
                newName: "UserProtheusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserProtheusId",
                table: "AspNetUsers",
                newName: "RaMatricula");
        }
    }
}
