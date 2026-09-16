using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineSpectra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRatingCountersAndScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AverageScore",
                table: "Shows",
                newName: "CriteriaAverageScore");

            migrationBuilder.AddColumn<int>(
                name: "CriteriaVoteCount",
                table: "Shows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "EpisodeAudienceScore",
                table: "Shows",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEpisodeVoteCount",
                table: "Shows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Seasons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShowId1",
                table: "MediaRatings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Episodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MediaRatings_ShowId1",
                table: "MediaRatings",
                column: "ShowId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaRatings_Shows_ShowId1",
                table: "MediaRatings",
                column: "ShowId1",
                principalTable: "Shows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaRatings_Shows_ShowId1",
                table: "MediaRatings");

            migrationBuilder.DropIndex(
                name: "IX_MediaRatings_ShowId1",
                table: "MediaRatings");

            migrationBuilder.DropColumn(
                name: "CriteriaVoteCount",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "EpisodeAudienceScore",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "TotalEpisodeVoteCount",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "ShowId1",
                table: "MediaRatings");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "CriteriaAverageScore",
                table: "Shows",
                newName: "AverageScore");
        }
    }
}
