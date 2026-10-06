using DesafioTecnico.Models;
using DesafioTecnico.Services;

namespace DesafioTecnico.Tests;

public class EstoqueServiceTests
{
    private readonly EstoqueService _service = new();

    private static Movimentacao Entrada(int id, int codigo, int quantidade) =>
        new(id, "Compra", TipoMovimentacao.Entrada, quantidade, codigo, DateTimeOffset.UnixEpoch);

    private static Movimentacao Saida(int id, int codigo, int quantidade) =>
        new(id, "Venda", TipoMovimentacao.Saida, quantidade, codigo, DateTimeOffset.UnixEpoch);

    private void CarregarProduto101()
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"estoque-{Guid.NewGuid():N}.json");

        File.WriteAllText(
            caminho,
            "{\"estoque\":[{\"codigoProduto\":101,\"descricaoProduto\":\"Caneta Azul\",\"estoque\":150}]}");

        _service.LerProdutos(caminho);

        File.Delete(caminho);
    }

    [Fact]
    public void RegistrarMovimentacao_Entrada_IncreaseOSaldo()
    {
        CarregarProduto101();

        var resultado = _service.RegistrarMovimentacao(Entrada(1, 101, 10));

        Assert.Equal(160, resultado.SaldoFinal);
    }

    [Fact]
    public void RegistrarMovimentacao_Saida_DiminuiOSaldo()
    {
        CarregarProduto101();

        var resultado = _service.RegistrarMovimentacao(Saida(1, 101, 10));

        Assert.Equal(140, resultado.SaldoFinal);
    }

    [Fact]
    public void RegistrarMovimentacao_SaidaMaiorQueOSaldo_DefineOComportamento()
    {
        CarregarProduto101();

        Assert.Throws<InvalidOperationException>(
            () => _service.RegistrarMovimentacao(Saida(1, 101, 999)));

        Assert.Equal(150, _service.ConsultarSaldo(101));
    }

    [Fact]
    public void RegistrarMovimentacao_IdRepetido_DefineOComportamento()
    {
        CarregarProduto101();

        _service.RegistrarMovimentacao(Entrada(1, 101, 10));

        Assert.Throws<InvalidOperationException>(
            () => _service.RegistrarMovimentacao(Entrada(1, 101, 5)));

        Assert.Equal(160, _service.ConsultarSaldo(101));
    }

    [Fact]
    public void RegistrarMovimentacao_ProdutoInexistente_DefineOComportamento()
    {
        CarregarProduto101();

        Assert.Throws<InvalidOperationException>(
            () => _service.RegistrarMovimentacao(Entrada(1, 999, 10)));
    }

    [Fact]
    public void ConsultarSaldo_SomaTodasAsMovimentacoes()
    {
        CarregarProduto101();

        _service.RegistrarMovimentacao(Entrada(1, 101, 30));
        _service.RegistrarMovimentacao(Saida(2, 101, 20));
        _service.RegistrarMovimentacao(Entrada(3, 101, 5));

        Assert.Equal(165, _service.ConsultarSaldo(101));
    }

    [Fact]
    public void ProximoId_NuncaRepete()
    {
        var ids = new List<int>();

        for (var i = 0; i < 10; i++)
        {
            ids.Add(_service.ProximoId());
        }

        Assert.Equal(10, ids.Distinct().Count());
    }
}
