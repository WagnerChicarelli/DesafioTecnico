namespace DesafioTecnico.Services;

public sealed class JurosService
{
    private const decimal TaxaDiaria = 0.025m;

    public int CalcularDiasDeAtraso(DateOnly vencimento, DateOnly hoje)
    {
        var dias = hoje.DayNumber - vencimento.DayNumber;

        if (dias < 0)
        {
            return 0;
        }

        return dias;
    }

    public decimal CalcularJuros(decimal valor, DateOnly vencimento, DateOnly hoje)
    {
        var dias = CalcularDiasDeAtraso(vencimento, hoje);

        return valor * TaxaDiaria * dias;
    }

    public decimal CalcularTotalComJuros(decimal valor, DateOnly vencimento, DateOnly hoje)
    {
        return valor + CalcularJuros(valor, vencimento, hoje);
    }
}
