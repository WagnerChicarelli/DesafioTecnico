using System.Text.Json.Serialization;

namespace DesafioTecnico.Models;

public sealed record Venda(string Vendedor, decimal Valor);

public sealed record ArquivoVendas
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; init; } = new();
}
