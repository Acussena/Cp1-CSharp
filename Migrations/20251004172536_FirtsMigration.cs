using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CheckPoint1.Migrations
{
    /// <inheritdoc />
    public partial class FirtsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CPF = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    CEP = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estoque = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroPedido = table.Column<string>(type: "TEXT", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PedidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "DataCriacao", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(7985), "Tecnologia e gadgets", "Eletrônicos" },
                    { 2, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(7996), "Moda masculina e feminina", "Roupas" },
                    { 3, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(7998), "Produtos alimentícios", "Alimentos" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Ativo", "CEP", "CPF", "Cidade", "DataCadastro", "Email", "Endereco", "Estado", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, true, null, null, null, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8143), "acussena@email.com", null, null, "Açussena", null },
                    { 2, true, null, null, null, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8145), "joao@email.com", null, null, "João Silva", null }
                });

            migrationBuilder.InsertData(
                table: "Pedidos",
                columns: new[] { "Id", "ClienteId", "DataPedido", "Desconto", "NumeroPedido", "Observacoes", "Status", "ValorTotal" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8168), null, "P001", null, 1, 3600m },
                    { 2, 2, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8170), null, "P002", null, 1, 245m }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Ativo", "CategoriaId", "DataCriacao", "Descricao", "Estoque", "Nome", "Preco" },
                values: new object[,]
                {
                    { 1, true, 1, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8110), null, 10, "Notebook", 3500m },
                    { 2, true, 1, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8112), null, 0, "Smartphone", 2500m },
                    { 3, true, 2, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8113), null, 100, "Camiseta", 50m },
                    { 4, true, 2, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8115), null, 50, "Calça Jeans", 120m },
                    { 5, true, 3, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8116), null, 200, "Arroz 5kg", 25m },
                    { 6, true, 3, new DateTime(2025, 10, 4, 14, 25, 36, 356, DateTimeKind.Local).AddTicks(8117), null, 0, "Feijão 1kg", 8m }
                });

            migrationBuilder.InsertData(
                table: "PedidoItens",
                columns: new[] { "Id", "Desconto", "PedidoId", "PrecoUnitario", "ProdutoId", "Quantidade" },
                values: new object[,]
                {
                    { 1, null, 1, 3500m, 1, 1 },
                    { 2, null, 1, 50m, 3, 2 },
                    { 3, null, 2, 25m, 5, 5 },
                    { 4, null, 2, 120m, 4, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_PedidoId",
                table: "PedidoItens",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_ProdutoId",
                table: "PedidoItens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteId",
                table: "Pedidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_NumeroPedido",
                table: "Pedidos",
                column: "NumeroPedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoItens");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
