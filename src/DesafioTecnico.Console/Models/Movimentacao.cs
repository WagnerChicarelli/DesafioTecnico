namespace DesafioTecnico.Models;

public enum TipoMovimentacao
{
    Entrada,
    Saida,
}

public sealed record Movimentacao(
    int Id,
    string Descricao,
    TipoMovimentacao Tipo,
    int Quantidade,
    int CodigoProduto,
    DateTimeOffset Data);
