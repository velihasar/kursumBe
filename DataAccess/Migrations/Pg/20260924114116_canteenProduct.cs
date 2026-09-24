using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class canteenProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CanteenProductId",
                table: "StudentWalletTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanteenProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Barcode = table.Column<string>(type: "text", nullable: true),
                    StockQuantity = table.Column<int>(type: "integer", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanteenProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanteenProducts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentWalletTransactions_CanteenProductId",
                table: "StudentWalletTransactions",
                column: "CanteenProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CanteenProducts_TenantId",
                table: "CanteenProducts",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentWalletTransactions_CanteenProducts_CanteenProductId",
                table: "StudentWalletTransactions",
                column: "CanteenProductId",
                principalTable: "CanteenProducts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentWalletTransactions_CanteenProducts_CanteenProductId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropTable(
                name: "CanteenProducts");

            migrationBuilder.DropIndex(
                name: "IX_StudentWalletTransactions_CanteenProductId",
                table: "StudentWalletTransactions");

            migrationBuilder.DropColumn(
                name: "CanteenProductId",
                table: "StudentWalletTransactions");
        }
    }
}
