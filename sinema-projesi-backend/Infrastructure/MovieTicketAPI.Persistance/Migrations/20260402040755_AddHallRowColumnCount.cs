using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHallRowColumnCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColumnCount",
                table: "Halls",
                type: "int",
                nullable: false,
                defaultValue: 12);

            migrationBuilder.AddColumn<int>(
                name: "RowCount",
                table: "Halls",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.Sql("UPDATE [Halls] SET [Capacity] = [RowCount] * [ColumnCount]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnCount",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "RowCount",
                table: "Halls");
        }
    }
}
