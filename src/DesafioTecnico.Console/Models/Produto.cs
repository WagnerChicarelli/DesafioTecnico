using System.Text.Json.Serialization;

namespace DesafioTecnico.Models;

public sealed record Produto(int CodigoProduto, string DescricaoProduto, int Estoque);

public sealed record ArquivoEstoque
{
    [JsonPropertyName("estoque")]
    public List<Produto> Produtos { get; init; } = new();
}
