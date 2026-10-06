using DesafioTecnico.Models;
using DesafioTecnico.Services;

namespace DesafioTecnico.Tests;

public class ComissaoServiceTests
{
    private readonly ComissaoService _service = new();

    [Fact]
    public void CalcularComissao_Valor99_99_RetornaZero()
    {
        var comissao = _service.CalcularComissao(99.99m);

        Assert.Equal(0m, comissao);
    }

    [Fact]
    public void CalcularComissao_Valor100_Retorna1()
    {
        var comissao = _service.CalcularComissao(100m);

        Assert.Equal(1m, comissao);
    }

    [Fact]
    public void CalcularComissao_Valor499_99_Retorna4_9999()
    {
        var comissao = _service.CalcularComissao(499.99m);

        Assert.Equal(4.9999m, comissao);
    }

    [Fact]
    public void CalcularComissao_Valor500_Retorna25()
    {
        var comissao = _service.CalcularComissao(500m);

        Assert.Equal(25m, comissao);
    }

    [Fact]
    public void CalcularComissao_Valor1000_Retorna50()
    {
        var comissao = _service.CalcularComissao(1000m);

        Assert.Equal(50m, comissao);
    }

    [Fact]
    public void CalcularPorVendedor_SomaComissaoDeCadaVenda_E_NaoSobreOTotal()
    {
        var vendas = new List<Venda>
        {
            new("Joao Silva", 600m),
            new("Joao Silva", 100m),
        };

        var resultado = _service.CalcularPorVendedor(vendas);
        var joao = resultado.Single(v => v.Vendedor == "Joao Silva");

        Assert.Equal(31m, joao.TotalComissao);
        Assert.Equal(700m, joao.TotalVendido);
        Assert.Equal(2, joao.QuantidadeVendas);
    }

    [Fact]
    public void CalcularPorVendedor_VendasDasTresFaixas_SomaOsTresPercentuais()
    {
        var vendas = new List<Venda>
        {
            new("Joao Silva", 50m),
            new("Joao Silva", 200m),
            new("Joao Silva", 1000m),
        };

        var resultado = _service.CalcularPorVendedor(vendas);
        var joao = resultado.Single(v => v.Vendedor == "Joao Silva");

        Assert.Equal(52m, joao.TotalComissao);
        Assert.Equal(1250m, joao.TotalVendido);
        Assert.Equal(3, joao.QuantidadeVendas);
    }
}
