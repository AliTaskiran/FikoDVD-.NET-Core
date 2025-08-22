using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Yalnızca DvdRentals tablosunun oluşturulması
            migrationBuilder.CreateTable(
                name: "DvdRentals",
                columns: table => new
                {
                    RentalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DvdId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RentalStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DvdRentals", x => x.RentalId);
                    table.ForeignKey(
                        name: "FK_DvdRentals_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DvdRentals_Dvds_DvdId",
                        column: x => x.DvdId,
                        principalTable: "Dvds",
                        principalColumn: "DvdId",
                        onDelete: ReferentialAction.Cascade);
                });

            // DvdRentals tablosu için index'ler
            migrationBuilder.CreateIndex(
                name: "IX_DvdRentals_ClientId",
                table: "DvdRentals",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DvdRentals_DvdId",
                table: "DvdRentals",
                column: "DvdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Yalnızca DvdRentals tablosunun silinmesi
            migrationBuilder.DropTable(
                name: "DvdRentals");
        }
    }
}
