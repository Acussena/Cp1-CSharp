using System.Data.SQLite;
using CheckPoint1.Models;

namespace CheckPoint1.Services;

public class AdoNetService
{
    private readonly string _connectionString;

    public AdoNetService()
    {
        // TODO: Implementar connection string para SQLite 
        // Usar o mesmo arquivo "loja.db" criado pelo EF
        _connectionString = "Data Source=loja.db;Version=3;";
    }

    // ========== CONSULTAS COMPLEXAS ==========

    public void RelatorioVendasCompleto()
    {
        // TODO: Implementar relatório com múltiplos JOINs 
        // SELECT com JOIN de 4 tabelas: Pedidos, Clientes, PedidoItens, Produtos
        // - Mostrar: NumeroPedido, NomeCliente, NomeProduto, Quantidade, PrecoUnitario, Subtotal
        // - Agrupar por pedido
        // - Ordenar por data do pedido

        Console.WriteLine("=== RELATÓRIO VENDAS COMPLETO (ADO.NET) ===");

        using var conn = GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.Id AS NumeroPedido, c.Nome AS NomeCliente, pr.Nome AS NomeProduto,
                   pi.Quantidade, pi.PrecoUnitario,
                   (pi.Quantidade * pi.PrecoUnitario) AS Subtotal
            FROM Pedidos p
            JOIN Clientes c ON c.Id = p.ClienteId
            JOIN PedidoItens pi ON pi.PedidoId = p.Id
            JOIN Produtos pr ON pr.Id = pi.ProdutoId
            ORDER BY p.DataPedido;";

        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"Pedido {reader["NumeroPedido"]} | Cliente: {reader["NomeCliente"]} | Produto: {reader["NomeProduto"]} | Qtd: {reader["Quantidade"]} | Unit: {reader["PrecoUnitario"]} | Subtotal: {reader["Subtotal"]}"
            );
        }
    }

    public void FaturamentoPorCliente()
    {
        // TODO: Implementar consulta com GROUP BY e SUM 
        // - Agrupar por cliente
        // - Calcular valor total de pedidos
        // - Contar quantidade de pedidos
        // - Calcular ticket médio
        // - Ordenar por faturamento decrescente

        Console.WriteLine("=== FATURAMENTO POR CLIENTE ===");

        using var conn = GetConnection();
        conn.Open();

        string sql = @"
            SELECT c.Nome AS NomeCliente,
                   SUM(pi.Quantidade * pi.PrecoUnitario) AS TotalFaturado,
                   COUNT(DISTINCT p.Id) AS QtdPedidos,
                   (SUM(pi.Quantidade * pi.PrecoUnitario) / COUNT(DISTINCT p.Id)) AS TicketMedio
            FROM Clientes c
            JOIN Pedidos p ON p.ClienteId = c.Id
            JOIN PedidoItens pi ON pi.PedidoId = p.Id
            GROUP BY c.Nome
            ORDER BY TotalFaturado DESC;";

        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"Cliente: {reader["NomeCliente"]} | Total: {reader["TotalFaturado"]} | Pedidos: {reader["QtdPedidos"]} | Ticket Médio: {reader["TicketMedio"]}"
            );
        }
    }

    public void ProdutosSemVenda()
    {
        // TODO: Implementar consulta com LEFT JOIN e IS NULL 
        // - Produtos que nunca foram vendidos
        // - Mostrar categoria, nome, preço, estoque
        // - Calcular valor parado em estoque

        Console.WriteLine("=== PRODUTOS SEM VENDAS ===");

        using var conn = GetConnection();
        conn.Open();

        string sql = @"
            SELECT pr.Nome AS NomeProduto, pr.Preco, pr.Estoque, pr.CategoriaId,
                   (pr.Preco * pr.Estoque) AS ValorParado
            FROM Produtos pr
            LEFT JOIN PedidoItens pi ON pi.ProdutoId = pr.Id
            WHERE pi.Id IS NULL;";

        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"Produto: {reader["NomeProduto"]} | CategoriaId: {reader["CategoriaId"]} | Preço: {reader["Preco"]} | Estoque: {reader["Estoque"]} | Valor parado: {reader["ValorParado"]}"
            );
        }
    }

    // ========== OPERAÇÕES DE DADOS ==========

    public void AtualizarEstoqueLote()
    {
        // TODO: Implementar UPDATE em lote com  
        // - Solicitar categoria e percentual de ajuste
        // - Atualizar estoque de todos produtos da categoria
        // - Exibir de quantos registros foram afetados

        Console.WriteLine("=== ATUALIZAR ESTOQUE EM LOTE ===");

        Console.Write("Informe a categoria (Id): ");
        int categoriaId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Informe o percentual de ajuste (+10 para aumentar, -10 para reduzir): ");
        double perc = double.Parse(Console.ReadLine() ?? "0");

        using var conn = GetConnection();
        conn.Open();

        string sql = "UPDATE Produtos SET Estoque = CAST(Estoque * (1 + @Perc/100.0) AS INT) WHERE CategoriaId = @CategoriaId";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Perc", perc);
        cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);

        int afetados = cmd.ExecuteNonQuery();
        Console.WriteLine($"Estoque atualizado em {afetados} produtos.");
    }

    public void InserirPedidoCompleto()
    {
        // TODO: Implementar INSERT com  
        // - Inserir pedido master - Pedido master é o que vai na tabela Pedidos.
        // - Inserir múltiplos itens - Pedido pode conter vários itens
        // - Atualizar estoque dos produtos
        // - Validar estoque antes de inserir o item no pedido
        // - Gerar NumeroPedido automaticamente

        Console.WriteLine("=== INSERIR PEDIDO COMPLETO ===");

        using var conn = GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        // ========== SELEÇÃO DO CLIENTE ==========
        Console.WriteLine("Clientes disponíveis:");
        var cmdClientes = new SQLiteCommand("SELECT Id, Nome FROM Clientes", conn, tx);
        using (var reader = cmdClientes.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"- {reader["Nome"]}");
            }
        }

        Console.Write("Digite o nome do cliente: ");
        string nomeCliente = Console.ReadLine() ?? "";

        var cmdClienteId = new SQLiteCommand("SELECT Id FROM Clientes WHERE Nome = @Nome", conn, tx);
        cmdClienteId.Parameters.AddWithValue("@Nome", nomeCliente);
        var clienteIdObj = cmdClienteId.ExecuteScalar();

        if (clienteIdObj == null)
        {
            Console.WriteLine($"\nErro: Cliente '{nomeCliente}' não encontrado! Cadastre o cliente antes de inserir o pedido.");
            return;
        }

        int clienteId = Convert.ToInt32(clienteIdObj);

        // ========== GERAR NUMEROPEDIDO AUTOMATICAMENTE ==========
        var cmdUltimoPedido = new SQLiteCommand("SELECT MAX(Id) FROM Pedidos", conn, tx);
        var ultimoIdObj = cmdUltimoPedido.ExecuteScalar();
        int ultimoId = ultimoIdObj != DBNull.Value ? Convert.ToInt32(ultimoIdObj) : 0;
        string numeroPedido = $"P{(ultimoId + 1):D3}";

        // ========== INSERIR PEDIDO MASTER ==========
        string insertPedido = "INSERT INTO Pedidos (NumeroPedido, ClienteId, DataPedido, Status, ValorTotal) " +
                              "VALUES (@NumeroPedido, @ClienteId, @Data, @Status, 0); SELECT last_insert_rowid();";
        using var cmdPedido = new SQLiteCommand(insertPedido, conn, tx);
        cmdPedido.Parameters.AddWithValue("@NumeroPedido", numeroPedido);
        cmdPedido.Parameters.AddWithValue("@ClienteId", clienteId);
        cmdPedido.Parameters.AddWithValue("@Data", DateTime.Now);
        cmdPedido.Parameters.AddWithValue("@Status", (int)StatusPedido.Confirmado);

        long pedidoId = (long)cmdPedido.ExecuteScalar();

        // ========== ADICIONAR ITENS ==========
        bool adicionarMais = true;
        double valorTotal = 0;

        while (adicionarMais)
        {
            Console.WriteLine("\nProdutos disponíveis:");
            var cmdProdutos = new SQLiteCommand("SELECT Id, Nome, Preco, Estoque FROM Produtos", conn, tx);
            using (var reader = cmdProdutos.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader["Id"]} | {reader["Nome"]} | Preço: {reader["Preco"]} | Estoque: {reader["Estoque"]}");
                }
            }

            Console.Write("Digite o Id do produto: ");
            int produtoId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Quantidade: ");
            int qtd = int.Parse(Console.ReadLine() ?? "0");

            // Valida estoque
            var checkEstoque = new SQLiteCommand("SELECT Estoque, Preco FROM Produtos WHERE Id=@Id", conn, tx);
            checkEstoque.Parameters.AddWithValue("@Id", produtoId);
            using var readerEstoque = checkEstoque.ExecuteReader();
            if (!readerEstoque.Read())
            {
                Console.WriteLine("Produto não encontrado!");
                continue;
            }
            int estoque = Convert.ToInt32(readerEstoque["Estoque"]);
            double preco = Convert.ToDouble(readerEstoque["Preco"]);
            readerEstoque.Close();

            if (estoque < qtd)
            {
                Console.WriteLine("Estoque insuficiente!");
                continue;
            }

            // Inserir item
            string insertItem = "INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) " +
                                "VALUES (@PedidoId, @ProdutoId, @Qtd, @Preco)";
            var cmdItem = new SQLiteCommand(insertItem, conn, tx);
            cmdItem.Parameters.AddWithValue("@PedidoId", pedidoId);
            cmdItem.Parameters.AddWithValue("@ProdutoId", produtoId);
            cmdItem.Parameters.AddWithValue("@Qtd", qtd);
            cmdItem.Parameters.AddWithValue("@Preco", preco);
            cmdItem.ExecuteNonQuery();

            // Atualizar estoque
            string updateEstoque = "UPDATE Produtos SET Estoque = Estoque - @Qtd WHERE Id=@ProdutoId";
            var cmdEst = new SQLiteCommand(updateEstoque, conn, tx);
            cmdEst.Parameters.AddWithValue("@Qtd", qtd);
            cmdEst.Parameters.AddWithValue("@ProdutoId", produtoId);
            cmdEst.ExecuteNonQuery();

            valorTotal += qtd * preco;

            Console.Write("Adicionar outro item? (s/n): ");
            adicionarMais = Console.ReadLine()?.ToLower() == "s";
        }

        // Atualizar valor total do pedido
        var cmdValorTotal = new SQLiteCommand("UPDATE Pedidos SET ValorTotal=@ValorTotal WHERE Id=@PedidoId", conn, tx);
        cmdValorTotal.Parameters.AddWithValue("@ValorTotal", valorTotal);
        cmdValorTotal.Parameters.AddWithValue("@PedidoId", pedidoId);
        cmdValorTotal.ExecuteNonQuery();

        tx.Commit();
        Console.WriteLine($"\nPedido {numeroPedido} inserido com sucesso! Total: {valorTotal:C2}");
    }


    public void ExcluirDadosAntigos()
    {
        // TODO: Implementar DELETE com subconsulta 
        // - Excluir pedidos cancelados há mais de 6 meses

        Console.WriteLine("=== EXCLUIR DADOS ANTIGOS ===");

        using var conn = GetConnection();
        conn.Open();

        string sql = @"
            DELETE FROM Pedidos
            WHERE Status = @StatusCancelado
            AND DataPedido < date('now', '-6 months');";

        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@StatusCancelado", (int)StatusPedido.Cancelado);

        int afetados = cmd.ExecuteNonQuery();
        Console.WriteLine($"Excluídos {afetados} pedidos cancelados há mais de 6 meses.");
    }

    public void CalcularComissaoPedido()
    {
        // TODO: Calcular comissão de um pedido específico
        // - Mostrar lista de pedidos com número e cliente
        // - Solicitar escolha do usuário
        // - Solicitar percentual de comissão
        // - Calcular comissão total do pedido (soma dos itens)

        Console.WriteLine("=== CALCULAR COMISSÃO DE PEDIDO ===");

        using var conn = GetConnection();
        conn.Open();

        // Mostrar lista de pedidos
        string sqlPedidos = @"
        SELECT p.Id, p.NumeroPedido, c.Nome AS NomeCliente
        FROM Pedidos p
        JOIN Clientes c ON c.Id = p.ClienteId
        ORDER BY p.DataPedido;";

        using var cmdPedidos = new SQLiteCommand(sqlPedidos, conn);
        using var reader = cmdPedidos.ExecuteReader();

        var pedidos = new List<(int Id, string NumeroPedido, string Cliente)>();

        Console.WriteLine("Pedidos disponíveis:");
        while (reader.Read())
        {
            int id = Convert.ToInt32(reader["Id"]);
            string numero = reader["NumeroPedido"].ToString() ?? "";
            string cliente = reader["NomeCliente"].ToString() ?? "";
            pedidos.Add((id, numero, cliente));
            Console.WriteLine($"- {numero} | Cliente: {cliente}");
        }
        reader.Close();

        // Solicitar pedido
        Console.Write("Digite o número do pedido para calcular a comissão: ");
        string numeroPedidoEscolhido = Console.ReadLine() ?? "";

        var pedidoSelecionado = pedidos.FirstOrDefault(p => p.NumeroPedido == numeroPedidoEscolhido);
        if (pedidoSelecionado.Id == 0)
        {
            Console.WriteLine("Pedido não encontrado!");
            return;
        }

        // Solicitar percentual de comissão
        Console.Write("Informe o percentual de comissão (ex: 5 para 5%): ");
        double percentual = double.Parse(Console.ReadLine() ?? "0");

        // Calcular comissão
        string sqlItens = @"
        SELECT pi.Quantidade, pi.PrecoUnitario
        FROM PedidoItens pi
        WHERE pi.PedidoId = @PedidoId;";

        using var cmdItens = new SQLiteCommand(sqlItens, conn);
        cmdItens.Parameters.AddWithValue("@PedidoId", pedidoSelecionado.Id);
        using var readerItens = cmdItens.ExecuteReader();

        double totalComissao = 0;

        while (readerItens.Read())
        {
            int qtd = Convert.ToInt32(readerItens["Quantidade"]);
            double preco = Convert.ToDouble(readerItens["PrecoUnitario"]);
            totalComissao += (qtd * preco) * (percentual / 100.0);
        }

        readerItens.Close();

        Console.WriteLine($"Comissão do pedido {pedidoSelecionado.NumeroPedido} (Cliente: {pedidoSelecionado.Cliente}): R$ {totalComissao:F2}");
    }


    public void ProcessarDevolucao()
    {
        // TODO: Implementar processo complexo 
        // 1. Localizar pedido e itens
        // 2. Validar se pode devolver
        // 3. Devolver estoque (no cadastro de produtos, aumentar o estoque de acordo com a quantidade do pedido.)

        Console.WriteLine("=== PROCESSAR DEVOLUÇÃO ===");

        Console.Write("Informe o Id do pedido: ");
        int pedidoId = int.Parse(Console.ReadLine() ?? "0");

        using var conn = GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        // Localizar pedido
        var cmdPed = new SQLiteCommand("SELECT Status FROM Pedidos WHERE Id=@Id", conn, tx);
        cmdPed.Parameters.AddWithValue("@Id", pedidoId);
        var status = cmdPed.ExecuteScalar();

        if (status == null)
        {
            Console.WriteLine("Pedido não encontrado!");
            return;
        }

        if ((int)(long)status != (int)StatusPedido.Entregue)
        {
            Console.WriteLine("Somente pedidos entregues podem ser devolvidos!");
            return;
        }

        // Itens
        var cmdItens = new SQLiteCommand("SELECT ProdutoId, Quantidade FROM PedidoItens WHERE PedidoId=@Id", conn, tx);
        cmdItens.Parameters.AddWithValue("@Id", pedidoId);
        using var reader = cmdItens.ExecuteReader();

        while (reader.Read())
        {
            int prodId = Convert.ToInt32(reader["ProdutoId"]);
            int qtd = Convert.ToInt32(reader["Quantidade"]);

            var cmdEst = new SQLiteCommand("UPDATE Produtos SET Estoque = Estoque + @Qtd WHERE Id=@ProdId", conn, tx);
            cmdEst.Parameters.AddWithValue("@Qtd", qtd);
            cmdEst.Parameters.AddWithValue("@ProdId", prodId);
            cmdEst.ExecuteNonQuery();
        }
        reader.Close();

        var cmdUpdate = new SQLiteCommand("UPDATE Pedidos SET Status=@Status WHERE Id=@Id", conn, tx);
        cmdUpdate.Parameters.AddWithValue("@Status", (int)StatusPedido.Cancelado);
        cmdUpdate.Parameters.AddWithValue("@Id", pedidoId);
        cmdUpdate.ExecuteNonQuery();

        tx.Commit();
        Console.WriteLine("Devolução processada com sucesso!");
    }

    // ========== ANÁLISES PERFORMANCE ==========

    public void AnalisarPerformanceVendas()
    {
        // TODO: Implementar análise
        // - Vendas mensais
        // - Crescimento percentual

        Console.WriteLine("=== ANÁLISE PERFORMANCE VENDAS ===");

        using var conn = GetConnection();
        conn.Open();

        string sql = @"
            SELECT strftime('%Y-%m', p.DataPedido) AS Mes,
                   SUM(pi.Quantidade * pi.PrecoUnitario) AS Total
            FROM Pedidos p
            JOIN PedidoItens pi ON pi.PedidoId = p.Id
            GROUP BY Mes
            ORDER BY Mes;";

        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        double? ultimo = null;
        while (reader.Read())
        {
            string mes = reader["Mes"].ToString() ?? "";
            double total = Convert.ToDouble(reader["Total"]);
            double crescimento = 0;
            if (ultimo != null && ultimo > 0)
                crescimento = ((total - ultimo.Value) / ultimo.Value) * 100;
            Console.WriteLine($"Mês: {mes} | Total: {total:F2} | Crescimento: {crescimento:F2}%");
            ultimo = total;
        }
    }

    // ========== UTILIDADES ==========

    private SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }

    public void TestarConexao()
    {
        // TODO: Implementar teste de conexão 
        // - Tentar conectar
        // - Executar query simples
        // - Mostrar informações do banco

        Console.WriteLine("=== TESTE DE CONEXÃO ===");

        using var conn = GetConnection();
        conn.Open();

        var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table';", conn);
        using var reader = cmd.ExecuteReader();

        Console.WriteLine("Tabelas existentes no banco:");
        while (reader.Read())
        {
            Console.WriteLine($"- {reader["name"]}");
        }
    }
}
