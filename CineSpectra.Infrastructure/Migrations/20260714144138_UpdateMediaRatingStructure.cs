using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CineSpectra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaRatingStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaRatings_RatingCriterias_CriteriaId",
                table: "MediaRatings");

            migrationBuilder.DropIndex(
                name: "IX_MediaRatings_CriteriaId",
                table: "MediaRatings");

            migrationBuilder.DropColumn(
                name: "CriteriaId",
                table: "MediaRatings");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "MediaRatings");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "RatingCriterias",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CalculatedRatingValue",
                table: "MediaRatings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "MediaRatingSubValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MediaRatingId = table.Column<int>(type: "integer", nullable: false),
                    CriteriaId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaRatingSubValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaRatingSubValues_MediaRatings_MediaRatingId",
                        column: x => x.MediaRatingId,
                        principalTable: "MediaRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MediaRatingSubValues_RatingCriterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "RatingCriterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaRatingSubValues_CriteriaId",
                table: "MediaRatingSubValues",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaRatingSubValues_MediaRatingId",
                table: "MediaRatingSubValues",
                column: "MediaRatingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaRatingSubValues");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "RatingCriterias");

            migrationBuilder.DropColumn(
                name: "CalculatedRatingValue",
                table: "MediaRatings");

            migrationBuilder.AddColumn<int>(
                name: "CriteriaId",
                table: "MediaRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "MediaRatings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MediaRatings_CriteriaId",
                table: "MediaRatings",
                column: "CriteriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaRatings_RatingCriterias_CriteriaId",
                table: "MediaRatings",
                column: "CriteriaId",
                principalTable: "RatingCriterias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
