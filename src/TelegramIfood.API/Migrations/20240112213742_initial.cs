using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramIfood.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estabelecimentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(255)", nullable: false),
                    IfoodMerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estabelecimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PedidosIfood",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    orderType = table.Column<string>(type: "varchar(255)", nullable: false),
                    orderTiming = table.Column<string>(type: "varchar(255)", nullable: false),
                    displayId = table.Column<string>(type: "varchar(255)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    preparationStartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isTest = table.Column<bool>(type: "bit", nullable: false),
                    salesChannel = table.Column<string>(type: "varchar(255)", nullable: false),
                    totalPedido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    pedidoStatus = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosIfood", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(255)", nullable: false),
                    IdTelegram = table.Column<long>(type: "bigint", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    EstabelecimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    index = table.Column<int>(type: "int", nullable: false),
                    uniqueId = table.Column<string>(type: "varchar(255)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    unit = table.Column<string>(type: "varchar(255)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    optionsPrice = table.Column<int>(type: "int", nullable: false),
                    totalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IfoodPedidoDetalheid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.id);
                    table.ForeignKey(
                        name: "FK_Item_PedidosIfood_IfoodPedidoDetalheid",
                        column: x => x.IfoodPedidoDetalheid,
                        principalTable: "PedidosIfood",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_IfoodPedidoDetalheid",
                table: "Item",
                column: "IfoodPedidoDetalheid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estabelecimentos");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "PedidosIfood");
        }
    }
}
