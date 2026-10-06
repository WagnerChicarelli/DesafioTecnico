using System.Text.Json;
using DesafioTecnico.Models;

namespace DesafioTecnico.Services;

public sealed record TotalPorVendedor(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal TotalComissao);

public sealed class ComissaoService
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public IReadOnlyList<Venda> LerVendas(string caminhoArquivo)
    {
        var json = File.ReadAllText(caminhoArquivo);
        var arquivo = JsonSerializer.Deserialize<ArquivoVendas>(json, OpcoesJson);

        if (arquivo is null)
        {
            throw new InvalidDataException("Arquivo de vendas vazio ou invalido.");
        }

        return arquivo.Vendas;
    }

    public decimal CalcularComissao(decimal valorDaVenda)
    {
        if (valorDaVenda < 100m)
        {
            return 0m;
        }

        if (valorDaVenda < 500m)
        {
            return valorDaVenda * 0.01m;
        }

        return valorDaVenda * 0.05m;
    }

    public IReadOnlyList<TotalPorVendedor> CalcularPorVendedor(IReadOnlyList<Venda> vendas)
    {
        var resultado = new List<TotalPorVendedor>();

        foreach (var grupo in vendas.GroupBy(v => v.Vendedor))
        {
            var totalVendido = grupo.Sum(v => v.Valor);
            var totalComissao = 0m;

            foreach (var venda in grupo)
            {
                totalComissao += CalcularComissao(venda.Valor);
            }

            resultado.Add(new TotalPorVendedor(
                grupo.Key,
                grupo.Count(),
                totalVendido,
                totalComissao));
        }

        return resultado;
    }
}
