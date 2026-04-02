using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketAPI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MovieCategories_RemoveMovieGenre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieCategories",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCategories", x => new { x.MovieId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_MovieCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieCategories_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieCategories_CategoryId",
                table: "MovieCategories",
                column: "CategoryId");

            migrationBuilder.Sql(@"
INSERT INTO [Categories] ([Name], [CreatedDate])
SELECT DISTINCT s.Part, GETDATE()
FROM (
    SELECT LTRIM(RTRIM(ss.value)) AS Part
    FROM [Movies] m
    CROSS APPLY STRING_SPLIT(REPLACE(ISNULL(m.[Genre], ''), ';', ','), ',') AS ss
    WHERE LTRIM(RTRIM(ISNULL(ss.value, ''))) <> ''
) s
WHERE NOT EXISTS (SELECT 1 FROM [Categories] c WHERE c.[Name] = s.Part);
");

            migrationBuilder.Sql(@"
INSERT INTO [MovieCategories] ([MovieId], [CategoryId])
SELECT m.[Id], c.[Id]
FROM [Movies] m
CROSS APPLY STRING_SPLIT(REPLACE(ISNULL(m.[Genre], ''), ';', ','), ',') AS ss
INNER JOIN [Categories] c ON c.[Name] = LTRIM(RTRIM(ss.value))
WHERE LTRIM(RTRIM(ISNULL(ss.value, ''))) <> '';
");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Movies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Movies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
UPDATE m
SET m.[Genre] = ISNULL((
    SELECT STRING_AGG(c.[Name], ', ')
    FROM [MovieCategories] mc
    INNER JOIN [Categories] c ON c.[Id] = mc.[CategoryId]
    WHERE mc.[MovieId] = m.[Id]
), '')
FROM [Movies] m;
");

            migrationBuilder.DropTable(
                name: "MovieCategories");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
