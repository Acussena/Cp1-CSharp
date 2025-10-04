using CheckPoint1.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckPoint1.Services;

    public class EntityFrameworkService
    {
        private readonly CheckpointContext _context;

        public EntityFrameworkService()
        {
            _context = new CheckpointContext();
        }

        // ========== CRUD CATEGORIAS ==========

        public void CriarCategoria()
        {
            // TODO: Implementar criação de categoria 
            Console.WriteLine("=== CRIAR CATEGORIA ===");
            Console.Write("Nome da categoria: ");
            var nome = Console.ReadLine();

            var categoria = new Categoria { Nome = nome! };
            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            Console.WriteLine("Categoria criada com sucesso!");
        }

        public void ListarCategorias()
        {
        // TODO: Implementar listagem com contagem de produtos 
            Console.WriteLine("=== CATEGORIAS ===");

            var categorias = _context.Categorias
                .Include(c => c.Produtos)
                .ToList();

            foreach (var c in categorias)
            {
                Console.WriteLine($"{c.Id} - {c.Nome} | Produtos: {c.Produtos.Count}");
            }
        }

        // ========== CRUD PRODUTOS ==========

        public void CriarProduto()
        {
            // TODO: Implementar criação de produto 
            // - Mostrar categorias disponíveis para o usuário escolher
            // - Validar categoria existe
            Console.WriteLine("=== CRIAR PRODUTO ===");

            ListarCategorias();
            Console.Write("Informe o ID da categoria: ");
            int catId = int.Parse(Console.ReadLine() ?? "0");

            var categoria = _context.Categorias.Find(catId);
            if (categoria == null)
            {
                Console.WriteLine("Categoria não encontrada!");
                return;
            }

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? "";
            Console.Write("Preço: ");
            decimal preco = decimal.Parse(Console.ReadLine() ?? "0");
            Console.Write("Estoque: ");
            int estoque = int.Parse(Console.ReadLine() ?? "0");

            var produto = new Produto { Nome = nome, Preco = preco, Estoque = estoque, CategoriaId = catId };
            _context.Produtos.Add(produto);
            _context.SaveChanges();

            Console.WriteLine("Produto criado!");
        }

        public void ListarProdutos()
        {
            // TODO: Implementar listagem com categoria 
            // - Usar Include para carregar categoria
            Console.WriteLine("=== PRODUTOS ===");

            var produtos = _context.Produtos
                .Include(p => p.Categoria)
                .ToList();

            foreach (var p in produtos)
            {
                Console.WriteLine($"{p.Id} - {p.Nome} | Categoria: {p.Categoria?.Nome} | Estoque: {p.Estoque} | Preço: {p.Preco}");
            }
        }

        public void AtualizarProduto()
        {
            // TODO: Implementar atualização de produto 
            Console.WriteLine("=== ATUALIZAR PRODUTO ===");

            ListarProdutos();
            Console.Write("ID do produto: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado!");
                return;
            }

            Console.Write("Novo nome (enter p/ manter): ");
            var nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome)) produto.Nome = nome;

            Console.Write("Novo preço (enter p/ manter): ");
            var precoStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(precoStr)) produto.Preco = decimal.Parse(precoStr);

            Console.Write("Novo estoque (enter p/ manter): ");
            var estoqueStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(estoqueStr)) produto.Estoque = int.Parse(estoqueStr);

            _context.SaveChanges();
            Console.WriteLine("Produto atualizado!");
        }

        // ========== CRUD CLIENTES ==========

        public void CriarCliente()
        {
            // TODO: Implementar criação de cliente 
            // - Validar email único
            // - Validar CPF formato - somente números devem ser armazenados
            Console.WriteLine("=== CRIAR CLIENTE ===");

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? "";
            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";
            Console.Write("CPF (somente números): ");
            string cpf = new string((Console.ReadLine() ?? "").Where(char.IsDigit).ToArray());

            if (_context.Clientes.Any(c => c.Email == email))
            {
                Console.WriteLine("Email já cadastrado!");
                return;
            }

            var cliente = new Cliente { Nome = nome, Email = email, CPF = cpf };
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            Console.WriteLine("Cliente criado!");
        }

        public void ListarClientes()
        {
            // TODO: Implementar listagem com contagem de pedidos 
            Console.WriteLine("=== CLIENTES ===");

            var clientes = _context.Clientes
                .Include(c => c.Pedidos)
                .ToList();

            foreach (var c in clientes)
            {
                Console.WriteLine($"{c.Id} - {c.Nome} | Email: {c.Email} | Pedidos: {c.Pedidos.Count}");
            }
        }

        public void AtualizarCliente()
        {
            // TODO: Implementar atualização de cliente 
            Console.WriteLine("=== ATUALIZAR CLIENTE ===");

            ListarClientes();
            Console.Write("ID do cliente: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
            {
                Console.WriteLine("Cliente não encontrado!");
                return;
            }

            Console.Write("Novo nome (enter p/ manter): ");
            var nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome)) cliente.Nome = nome;

            Console.Write("Novo email (enter p/ manter): ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (_context.Clientes.Any(c => c.Email == email && c.Id != id))
                {
                    Console.WriteLine("Email já cadastrado!");
                    return;
                }
                cliente.Email = email;
            }

            _context.SaveChanges();
            Console.WriteLine("Cliente atualizado!");
        }

        // ========== CRUD PEDIDOS ==========

        public void CriarPedido()
        {
            // TODO: Implementar criação de pedido completo 
            // - Pedir o ID do cliente
            // - Gerar número do pedido automático
            // - Permitir adicionar múltiplos itens
            // - Calcular valor total automaticamente
            // - Validar estoque disponível
            Console.WriteLine("=== CRIAR PEDIDO ===");

            ListarClientes();
            Console.Write("Cliente ID: ");
            int clienteId = int.Parse(Console.ReadLine() ?? "0");

            var cliente = _context.Clientes.Find(clienteId);
            if (cliente == null)
            {
                Console.WriteLine("Cliente não encontrado!");
                return;
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now,
                NumeroPedido = Guid.NewGuid().ToString().Substring(0, 8),
                Status = StatusPedido.Pendente,
                Itens = new List<PedidoItem>()
            };

            bool adicionarMais = true;
            while (adicionarMais)
            {
                ListarProdutos();
                Console.Write("Produto ID: ");
                int prodId = int.Parse(Console.ReadLine() ?? "0");

                var produto = _context.Produtos.Find(prodId);
                if (produto == null)
                {
                    Console.WriteLine("Produto não encontrado!");
                    continue;
                }

                Console.Write("Quantidade: ");
                int qtd = int.Parse(Console.ReadLine() ?? "0");
                if (produto.Estoque < qtd)
                {
                    Console.WriteLine("Estoque insuficiente!");
                    continue;
                }

                pedido.Itens.Add(new PedidoItem
                {
                    ProdutoId = prodId,
                    Quantidade = qtd,
                    PrecoUnitario = produto.Preco
                });

                produto.Estoque -= qtd;

                Console.Write("Adicionar outro item? (s/n): ");
                adicionarMais = Console.ReadLine()?.ToLower() == "s";
            }

            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            Console.WriteLine("Pedido criado!");
        }

        public void ListarPedidos()
        {
            // TODO: Implementar listagem com cliente e itens 
            // - Usar Include para carregar Cliente e Itens
            // - Incluir Produtos nos itens
            Console.WriteLine("=== PEDIDOS ===");

            var pedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens).ThenInclude(i => i.Produto)
                .ToList();

            foreach (var p in pedidos)
            {
                Console.WriteLine($"Pedido {p.Id} - Nº {p.NumeroPedido} | Cliente: {p.Cliente?.Nome} | Status: {p.Status}");
                foreach (var i in p.Itens)
                {
                    Console.WriteLine($"   Produto: {i.Produto?.Nome} | Qtd: {i.Quantidade} | Unit: {i.PrecoUnitario}");
                }
            }
        }

        public void AtualizarStatusPedido()
        {
            // TODO: Implementar mudança de status 
            // - Mostrar status disponíveis
            // - Validar transições válidas
            Console.WriteLine("=== ATUALIZAR STATUS PEDIDO ===");

            ListarPedidos();
            Console.Write("Pedido ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var pedido = _context.Pedidos.Find(id);
            if (pedido == null)
            {
                Console.WriteLine("Pedido não encontrado!");
                return;
            }

            Console.WriteLine("Status disponíveis:");
            foreach (var s in Enum.GetValues<StatusPedido>())
                Console.WriteLine($"- {(int)s}: {s}");

            Console.Write("Novo status: ");
            int novo = int.Parse(Console.ReadLine() ?? "0");

            pedido.Status = (StatusPedido)novo;
            _context.SaveChanges();

            Console.WriteLine("Status atualizado!");
        }

        public void CancelarPedido()
        {
            // TODO: Implementar cancelamento 
            // - Pedir id do pedido
            // - Validar se o pedido existe
            // - Só permite cancelar se status = Pendente ou Confirmado
            // - Devolver estoque dos produtos
            Console.WriteLine("=== CANCELAR PEDIDO ===");

            ListarPedidos();
            Console.Write("Pedido ID: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var pedido = _context.Pedidos
                .Include(p => p.Itens).ThenInclude(i => i.Produto)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                Console.WriteLine("Pedido não encontrado!");
                return;
            }

            if (pedido.Status != StatusPedido.Pendente && pedido.Status != StatusPedido.Confirmado)
            {
                Console.WriteLine("Pedido não pode ser cancelado!");
                return;
            }

            foreach (var item in pedido.Itens)
            {
                if (item.Produto != null)
                    item.Produto.Estoque += item.Quantidade;
            }

            pedido.Status = StatusPedido.Cancelado;
            _context.SaveChanges();

            Console.WriteLine("Pedido cancelado!");
        }

        // ========== CONSULTAS LINQ AVANÇADAS ==========

        public void ConsultasAvancadas()
        {
            Console.WriteLine("=== CONSULTAS LINQ ===");
            Console.WriteLine("1. Produtos mais vendidos");
            Console.WriteLine("2. Clientes com mais pedidos");
            Console.WriteLine("3. Faturamento por categoria");
            Console.WriteLine("4. Pedidos por período");
            Console.WriteLine("5. Produtos em estoque baixo");
            

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": ProdutosMaisVendidos(); break;
                case "2": ClientesComMaisPedidos(); break;
                case "3": FaturamentoPorCategoria(); break;
                case "4": PedidosPorPeriodo(); break;
                case "5": ProdutosEstoqueBaixo(); break;
                case "6": AnaliseVendasMensal(); break;
                case "7": TopClientesPorValor(); break;
            }
        }

        private void ProdutosMaisVendidos()
        {
            // TODO: Implementar consulta LINQ 
            // - Agrupar por produto
            // - Somar quantidades vendidas
            // - Ordenar por quantidade decrescente
            // - Incluir nome produto e categoria
            var query = _context.PedidoItens
            .Include(i => i.Produto).ThenInclude(p => p.Categoria)
            .GroupBy(i => i.Produto)
            .Select(g => new
            {
                Produto = g.Key!.Nome,
                Categoria = g.Key.Categoria!.Nome,
                Total = g.Sum(i => i.Quantidade)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Produto} ({r.Categoria}) - Vendidos: {r.Total}");
        }

        private void ClientesComMaisPedidos()
        {
            // TODO: Implementar consulta LINQ 
            // - Agrupar por cliente
            // - Contar pedidos
            // - Ordenar por quantidade decrescente
            var query = _context.Pedidos
            .GroupBy(p => p.Cliente!)
            .Select(g => new
            {
                Cliente = g.Key.Nome,
                Qtd = g.Count()
            })
            .OrderByDescending(x => x.Qtd)
            .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Cliente} - Pedidos: {r.Qtd}");
        }

        private void FaturamentoPorCategoria()
        {
            // TODO: Implementar consulta LINQ 
            // - Agrupar por categoria
            // - Calcular valor total vendido
            // - Contar produtos vendidos
            // - Calcular ticket médio
            var query = _context.PedidoItens
            .Include(i => i.Produto).ThenInclude(p => p.Categoria)
            .GroupBy(i => i.Produto!.Categoria!)
            .Select(g => new
            {
                Categoria = g.Key.Nome,
                Total = g.Sum(i => i.Quantidade * i.PrecoUnitario),
                Qtd = g.Count()
            })
            .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Categoria} - Faturamento: {r.Total} | Itens: {r.Qtd}");
        }

        private void PedidosPorPeriodo()
        {
            // TODO: Implementar consulta LINQ 
            // - Solicitar data início e fim
            // - Filtrar de pedidos por período
            // - Agrupar por data
            // - Calcular totais
            Console.Write("Data início (yyyy-MM-dd): ");
            DateTime inicio = DateTime.Parse(Console.ReadLine() ?? "");
            Console.Write("Data fim (yyyy-MM-dd): ");
            DateTime fim = DateTime.Parse(Console.ReadLine() ?? "");

            var query = _context.Pedidos
                .Where(p => p.DataPedido >= inicio && p.DataPedido <= fim)
                .GroupBy(p => p.DataPedido.Date)
                .Select(g => new
                {
                    Data = g.Key,
                    Total = g.Sum(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario))
                })
                .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Data:dd/MM/yyyy} - Total: {r.Total}");
        }

        private void ProdutosEstoqueBaixo()
        {
            // TODO: Implementar consulta LINQ 
            // - Filtrar produtos com estoque < 20
            // - Incluir categoria
            // - Ordenar por estoque crescente
            var query = _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Estoque < 20)
            .OrderBy(p => p.Estoque)
            .ToList();

            foreach (var p in query)
                Console.WriteLine($"{p.Nome} | Categoria: {p.Categoria?.Nome} | Estoque: {p.Estoque}");
        }

        private void AnaliseVendasMensal()
        {
            // TODO: Implementar consulta LINQ 
            // - Agrupar vendas por mês/ano
            // - Calcular quantidade vendida, faturamento
            // - Comparar com mês anterior
            var query = _context.PedidoItens
            .Include(i => i.Pedido)
            .GroupBy(i => new { i.Pedido!.DataPedido.Year, i.Pedido.DataPedido.Month })
            .Select(g => new
            {
                Mes = $"{g.Key.Month}/{g.Key.Year}",
                Total = g.Sum(i => i.Quantidade * i.PrecoUnitario)
            })
            .OrderBy(x => x.Mes)
            .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Mes} - Total: {r.Total}");
        }

        private void TopClientesPorValor()
        {
            var query = _context.Pedidos
            .GroupBy(p => p.Cliente!)
            .Select(g => new
            {
                Cliente = g.Key.Nome,
                Total = g.Sum(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario))
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToList();

            foreach (var r in query)
                Console.WriteLine($"{r.Cliente} - Total: {r.Total}");
        }

        // ========== RELATÓRIOS GERAIS ==========

        public void RelatoriosGerais()
        {
            Console.WriteLine("=== RELATÓRIOS GERAIS ===");
            Console.WriteLine("1. Dashboard executivo");
            Console.WriteLine("2. Relatório de estoque");
            Console.WriteLine("3. Análise de clientes");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": DashboardExecutivo(); break;
                case "2": RelatorioEstoque(); break;
                case "3": AnaliseClientes(); break;
            }
        }

        private void DashboardExecutivo()
        {
            // TODO: Implementar dashboard completo
            // - Quantidade de pedidos
            // - Ticket médio
            // - Produtos em estoque
            // - Clientes ativos
            // - Faturamento mensal
            int qtdPedidos = _context.Pedidos.Count();
            decimal ticketMedio = qtdPedidos > 0
                ? _context.PedidoItens.Sum(i => i.Quantidade * i.PrecoUnitario) / qtdPedidos
                : 0;
            int estoque = _context.Produtos.Sum(p => p.Estoque);
            int clientesAtivos = _context.Clientes.Count();
            decimal faturamentoMensal = _context.Pedidos
                .Where(p => p.DataPedido.Month == DateTime.Now.Month && p.DataPedido.Year == DateTime.Now.Year)
                .Sum(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario));

            Console.WriteLine($"Pedidos: {qtdPedidos}");
            Console.WriteLine($"Ticket Médio: {ticketMedio}");
            Console.WriteLine($"Estoque total: {estoque}");
            Console.WriteLine($"Clientes ativos: {clientesAtivos}");
            Console.WriteLine($"Faturamento mensal: {faturamentoMensal}");
        }

        private void RelatorioEstoque()
        {
            // TODO: Implementar relatório de estoque 
            // - Produtos por categoria
            // - Valor total em estoque
            // - Produtos zerados
            // - Produtos em estoque baixo
            var categorias = _context.Categorias.Include(c => c.Produtos).ToList();
            foreach (var c in categorias)
            {
                var totalEstoque = c.Produtos.Sum(p => p.Estoque * p.Preco);
                var zerados = c.Produtos.Count(p => p.Estoque == 0);
                var baixos = c.Produtos.Count(p => p.Estoque < 20);

                Console.WriteLine($"{c.Nome} - Valor total em estoque: {totalEstoque} | Zerados: {zerados} | Baixos: {baixos}");
            }
        }

        private void AnaliseClientes()
        {
        // TODO: Implementar análise de clientes 
        // - Clientes por estado
        // - Valor médio por cliente
        var clientes = _context.Clientes
            .Include(c => c.Pedidos)
                .ThenInclude(p => p.Itens)
            .ToList();

            var query = clientes
                .GroupBy(c => c.Email.Split('@').Last()) 
                .Select(g => new
                {
                    Dominio = g.Key,
                    Qtd = g.Count(),
                    ValorMedio = g.Average(c => c.Pedidos.Sum(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)))
                })
                .ToList();

            foreach (var r in query)
                Console.WriteLine($"Dominio: {r.Dominio} | Clientes: {r.Qtd} | Valor médio: {r.ValorMedio}");
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
