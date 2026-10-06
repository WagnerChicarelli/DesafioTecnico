using System.Globalization;
using System.Text;
using DesafioTecnico.Models;
using DesafioTecnico.Services;

namespace DesafioTecnico;

internal static class Program
{
    private static readonly string CaminhoVendas =
        Path.Combine(AppContext.BaseDirectory, "Data", "vendas.json");

    private static readonly string CaminhoEstoque =
        Path.Combine(AppContext.BaseDirectory, "Data", "estoque.json");
    private const string FormatoLinha = "{0,-24}{1,6}{2,18:N2}{3,18:N2}";

    private static readonly ComissaoService Comissao = new();
    private static readonly EstoqueService Estoque = new();
    private static readonly JurosService Juros = new();

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

        while (true)
        {
            Titulo("DESAFIO TECNICO");

            Console.WriteLine("1 - Comissao de vendas");
            Console.WriteLine("2 - Movimentacoes de estoque");
            Console.WriteLine("3 - Juros por atraso");
            Console.WriteLine("0 - Sair");
            Console.WriteLine();

            var opcao = Console.ReadLine()?.Trim();

            switch (opcao)
            {
                case "1":
                    Desafio1_Comissao();
                    break;
                case "2":
                    Desafio2_Estoque();
                    break;
                case "3":
                    Desafio3_Juros();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opcao invalida.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Pressione Enter para voltar ao menu.");
            Console.ReadLine();
        }
    }

    private static void Desafio1_Comissao()
    {
        Titulo("DESAFIO 1 - COMISSAO DE VENDAS");

        var vendas = Comissao.LerVendas(CaminhoVendas);
        var totais = Comissao.CalcularPorVendedor(vendas);

        Console.WriteLine(string.Format(FormatoLinha, "Vendedor", "Vendas", "Total vendido", "Comissao"));
        Console.WriteLine(new string('-', 66));

        foreach (var total in totais)
        {
            Console.WriteLine(string.Format(FormatoLinha, total.Vendedor, total.QuantidadeVendas, total.TotalVendido, total.TotalComissao));
        }

        Console.WriteLine(new string('-', 66));
        Console.WriteLine(string.Format(
            FormatoLinha,
            "TOTAL",
            totais.Sum(total => total.QuantidadeVendas),
            totais.Sum(total => total.TotalVendido),
            totais.Sum(total => total.TotalComissao)));
    }

    private static void Desafio2_Estoque()
    {
        Titulo("DESAFIO 2 - MOVIMENTACOES DE ESTOQUE");

        var produtos = Estoque.LerProdutos(CaminhoEstoque);

        foreach (var produto in produtos)
        {
            Console.WriteLine($"{produto.CodigoProduto} - {produto.DescricaoProduto,-30} {produto.Estoque,6}");
        }

        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("1 - Entrar mercadoria");
            Console.WriteLine("2 - Sair mercadoria");
            Console.WriteLine("3 - Consultar saldo");
            Console.WriteLine("0 - Voltar");
            Console.WriteLine();

            var opcao = Console.ReadLine()?.Trim();

            switch (opcao)
            {
                case "1":
                case "2":
                    var resultado = Estoque.RegistrarMovimentacao(MontarMovimentacao(entrada: opcao == "1"));
                    Console.WriteLine($"Movimentacao {resultado.Movimentacao.Id} registrada. Saldo final: {resultado.SaldoFinal}");
                    break;
                case "3":
                    Console.WriteLine($"Saldo: {Estoque.ConsultarSaldo(LerInteiro("Codigo do produto: "))}");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opcao invalida.");
                    break;
            }
        }
    }

    private static void Desafio3_Juros()
    {
        Titulo("DESAFIO 3 - JUROS POR ATRASO");

        Console.Write("Valor da divida: R$ ");
        var valor = decimal.Parse(Console.ReadLine() ?? "0", CultureInfo.CurrentCulture);

        Console.Write("Data de vencimento (AAAA-MM-DD): ");
        var vencimento = DateOnly.ParseExact(Console.ReadLine() ?? "", "yyyy-MM-dd", CultureInfo.CurrentCulture);

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        Console.WriteLine();
        Console.WriteLine($"Dias de atraso: {Juros.CalcularDiasDeAtraso(vencimento, hoje)}");
        Console.WriteLine($"Juros: R$ {Juros.CalcularJuros(valor, vencimento, hoje):N2}");
        Console.WriteLine($"Total: R$ {Juros.CalcularTotalComJuros(valor, vencimento, hoje):N2}");
    }

    private static Movimentacao MontarMovimentacao(bool entrada)
    {
        Console.Write("Codigo do produto: ");
        var codigo = LerInteiro(string.Empty);

        Console.Write("Quantidade: ");
        var quantidade = LerInteiro(string.Empty);

        Console.Write("Descricao da movimentacao: ");
        var descricao = Console.ReadLine() ?? string.Empty;

        return new Movimentacao(
            Estoque.ProximoId(),
            descricao,
            entrada ? TipoMovimentacao.Entrada : TipoMovimentacao.Saida,
            quantidade,
            codigo,
            DateTimeOffset.Now);
    }

    private static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);

            if (int.TryParse(Console.ReadLine(), out var valor))
            {
                return valor;
            }

            Console.WriteLine("Digite um numero inteiro.");
        }
    }

    private static void Titulo(string texto)
    {
        Console.WriteLine();
        Console.WriteLine(texto);
        Console.WriteLine(new string('=', 66));
    }
}
