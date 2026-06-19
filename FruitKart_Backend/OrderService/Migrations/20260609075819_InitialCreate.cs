using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_OrderHeader",
                columns: table => new
                {
                    OrderHeaderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PickUpName = table.Column<string>(type: "text", nullable: false),
                    PickUpPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PickUpEmail = table.Column<string>(type: "text", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    OrderTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TotalItem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_OrderHeader", x => x.OrderHeaderId);
                });

            migrationBuilder.CreateTable(
                name: "tbl_OrderDetail",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderHeaderId = table.Column<int>(type: "integer", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_OrderDetail", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_tbl_OrderDetail_tbl_OrderHeader_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "tbl_OrderHeader",
                        principalColumn: "OrderHeaderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_OrderDetail_OrderHeaderId",
                table: "tbl_OrderDetail",
                column: "OrderHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_OrderDetail");

            migrationBuilder.DropTable(
                name: "tbl_OrderHeader");
        }
    }
}
