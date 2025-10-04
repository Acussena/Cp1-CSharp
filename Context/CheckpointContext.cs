using CheckPoint1.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckPoint1;

public class CheckpointContext : DbContext
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Conexão com SQLite
        optionsBuilder.UseSqlite("Data Source=loja.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Categoria -> Produtos (1:N, Cascade)
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cliente -> Pedidos (1:N, Cascade)
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Cliente)
            .WithMany(c => c.Pedidos)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Pedido -> PedidoItens (1:N, Cascade)
        modelBuilder.Entity<PedidoItem>()
            .HasOne(pi => pi.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(pi => pi.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Produto -> PedidoItens (1:N, Restrict)
        modelBuilder.Entity<PedidoItem>()
            .HasOne(pi => pi.Produto)
            .WithMany(p => p.PedidoItens)
            .HasForeignKey(pi => pi.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices únicos
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Pedido>()
            .HasIndex(p => p.NumeroPedido)
            .IsUnique();

        // SEED DATA
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Eletrônicos", Descricao = "Tecnologia e gadgets", DataCriacao = DateTime.Now },
            new Categoria { Id = 2, Nome = "Roupas", Descricao = "Moda masculina e feminina", DataCriacao = DateTime.Now },
            new Categoria { Id = 3, Nome = "Alimentos", Descricao = "Produtos alimentícios", DataCriacao = DateTime.Now }
        );

        modelBuilder.Entity<Produto>().HasData(
            new Produto { Id = 1, Nome = "Notebook", Preco = 3500, Estoque = 10, CategoriaId = 1, DataCriacao = DateTime.Now },
            new Produto { Id = 2, Nome = "Smartphone", Preco = 2500, Estoque = 0, CategoriaId = 1, DataCriacao = DateTime.Now },
            new Produto { Id = 3, Nome = "Camiseta", Preco = 50, Estoque = 100, CategoriaId = 2, DataCriacao = DateTime.Now },
            new Produto { Id = 4, Nome = "Calça Jeans", Preco = 120, Estoque = 50, CategoriaId = 2, DataCriacao = DateTime.Now },
            new Produto { Id = 5, Nome = "Arroz 5kg", Preco = 25, Estoque = 200, CategoriaId = 3, DataCriacao = DateTime.Now },
            new Produto { Id = 6, Nome = "Feijão 1kg", Preco = 8, Estoque = 0, CategoriaId = 3, DataCriacao = DateTime.Now }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nome = "Açussena", Email = "acussena@email.com", DataCadastro = DateTime.Now },
            new Cliente { Id = 2, Nome = "João Silva", Email = "joao@email.com", DataCadastro = DateTime.Now }
        );

        modelBuilder.Entity<Pedido>().HasData(
            new Pedido { Id = 1, NumeroPedido = "P001", DataPedido = DateTime.Now, ClienteId = 1, ValorTotal = 3600 },
            new Pedido { Id = 2, NumeroPedido = "P002", DataPedido = DateTime.Now, ClienteId = 2, ValorTotal = 245 }
        );

        modelBuilder.Entity<PedidoItem>().HasData(
            new PedidoItem { Id = 1, PedidoId = 1, ProdutoId = 1, Quantidade = 1, PrecoUnitario = 3500 },
            new PedidoItem { Id = 2, PedidoId = 1, ProdutoId = 3, Quantidade = 2, PrecoUnitario = 50 },
            new PedidoItem { Id = 3, PedidoId = 2, ProdutoId = 5, Quantidade = 5, PrecoUnitario = 25 },
            new PedidoItem { Id = 4, PedidoId = 2, ProdutoId = 4, Quantidade = 1, PrecoUnitario = 120 }
        );
    }
}
