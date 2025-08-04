using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedSelectors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceDetailQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceEarlyPromotionQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceGiftPromotionQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceHighlightReviewQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceReviewerNameQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceTagQueryParams",
                table: "CrawlConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceDetailQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceEarlyPromotionQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceGiftPromotionQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceHighlightReviewQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceReviewerNameQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceTagQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
