using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChunkIndexToAnalyzeResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "confidence",
                table: "analyze_results",
                type: "numeric(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<int>(
                name: "chunk_index",
                table: "analyze_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_analyze_results_session_id_chunk_index",
                table: "analyze_results",
                columns: new[] { "session_id", "chunk_index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_analyze_results_session_id_chunk_index",
                table: "analyze_results");

            migrationBuilder.DropColumn(
                name: "chunk_index",
                table: "analyze_results");

            migrationBuilder.AlterColumn<decimal>(
                name: "confidence",
                table: "analyze_results",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,2)",
                oldPrecision: 3,
                oldScale: 2);
        }
    }
}
