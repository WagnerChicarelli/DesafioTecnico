using DesafioTecnico.Services;

namespace DesafioTecnico.Tests;

public class JurosServiceTests
{
    private readonly JurosService _service = new();

    [Fact]
    public void CalcularDiasDeAtraso_VencimentoHoje_RetornaZero()
    {
        var hoje = new DateOnly(2026, 10, 6);

        var dias = _service.CalcularDiasDeAtraso(hoje, hoje);

        Assert.Equal(0, dias);
    }

    [Fact]
    public void CalcularDiasDeAtraso_VencimentoAmanha_RetornaZero()
    {
        var vencimento = new DateOnly(2026, 10, 7);
        var hoje = new DateOnly(2026, 10, 6);

        var dias = _service.CalcularDiasDeAtraso(vencimento, hoje);

        Assert.Equal(0, dias);
    }

    [Fact]
    public void CalcularDiasDeAtraso_VencimentoOntem_RetornaUm()
    {
        var vencimento = new DateOnly(2026, 10, 5);
        var hoje = new DateOnly(2026, 10, 6);

        var dias = _service.CalcularDiasDeAtraso(vencimento, hoje);

        Assert.Equal(1, dias);
    }

    [Fact]
    public void CalcularDiasDeAtraso_NaoDependeDaHoraDoDia()
    {
        var vencimento = DateOnly.FromDateTime(new DateTime(2026, 10, 5, 23, 59, 59));
        var hoje = DateOnly.FromDateTime(new DateTime(2026, 10, 6, 0, 0, 1));

        var comHora = _service.CalcularDiasDeAtraso(vencimento, hoje);
        var semHora = _service.CalcularDiasDeAtraso(new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 6));

        Assert.Equal(1, comHora);
        Assert.Equal(semHora, comHora);
    }

    [Fact]
    public void CalcularJuros_Valor1000_TrintaDias_ConfereFormulaDefinida()
    {
        var vencimento = new DateOnly(2026, 10, 6);
        var hoje = new DateOnly(2026, 11, 5);

        var juros = _service.CalcularJuros(1000m, vencimento, hoje);

        Assert.Equal(750m, juros);
    }

    [Fact]
    public void CalcularJuros_VencimentoNoFuturo_RetornaZero()
    {
        var vencimento = new DateOnly(2026, 12, 25);
        var hoje = new DateOnly(2026, 10, 6);

        var juros = _service.CalcularJuros(1000m, vencimento, hoje);

        Assert.Equal(0m, juros);
    }

    [Fact]
    public void CalcularTotalComJuros_E_SomaDoValorComOsJuros()
    {
        var vencimento = new DateOnly(2026, 10, 6);
        var hoje = new DateOnly(2026, 10, 16);
        const decimal valor = 500m;

        var juros = _service.CalcularJuros(valor, vencimento, hoje);
        var total = _service.CalcularTotalComJuros(valor, vencimento, hoje);

        Assert.Equal(125m, juros);
        Assert.Equal(valor + juros, total);
    }
}
