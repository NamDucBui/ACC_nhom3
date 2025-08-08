using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetPostingSlug",
                table: "CrawlConfigs",
                newName: "SourceTagQueryParams");

            migrationBuilder.RenameColumn(
                name: "SourceType",
                table: "CrawlConfigs",
                newName: "SourceReviewerNameQueryParams");

            migrationBuilder.RenameColumn(
                name: "SourceBaseDomain",
                table: "CrawlConfigs",
                newName: "SourceRatingScoreQueryParams");

            migrationBuilder.AddColumn<string>(
                name: "SourceDurationQueryParams",
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
                name: "SourcePriceOriginalQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourcePricePromotionQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceRatingCountQueryParams",
                table: "CrawlConfigs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceDurationQueryParams",
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
                name: "SourcePriceOriginalQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourcePricePromotionQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.DropColumn(
                name: "SourceRatingCountQueryParams",
                table: "CrawlConfigs");

            migrationBuilder.RenameColumn(
                name: "SourceTagQueryParams",
                table: "CrawlConfigs",
                newName: "TargetPostingSlug");

            migrationBuilder.RenameColumn(
                name: "SourceReviewerNameQueryParams",
                table: "CrawlConfigs",
                newName: "SourceType");

            migrationBuilder.RenameColumn(
                name: "SourceRatingScoreQueryParams",
                table: "CrawlConfigs",
                newName: "SourceBaseDomain");
        }
    }
}
