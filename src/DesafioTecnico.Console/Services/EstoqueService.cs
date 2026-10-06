using System.Text.Json;
using DesafioTecnico.Models;

namespace DesafioTecnico.Services;

public sealed record ResultadoMovimentacao(Movimentacao Movimentacao, int SaldoFinal);

public sealed class EstoqueService
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly List<Produto> _produtos = new();
    private readonly List<Movimentacao> _movimentacoes = new();

    private int _proximoId = 1;

    public IReadOnlyList<Produto> LerProdutos(string caminhoArquivo)
    {
        var json = File.ReadAllText(caminhoArquivo);
        var arquivo = JsonSerializer.Deserialize<ArquivoEstoque>(json, OpcoesJson);

        if (arquivo is null)
        {
            throw new InvalidDataException("Arquivo de estoque vazio ou invalido.");
        }

        _produtos.Clear();
        _produtos.AddRange(arquivo.Produtos);

        return _produtos;
    }

    public ResultadoMovimentacao RegistrarMovimentacao(Movimentacao movimentacao)
    {
        var produto = _produtos.FirstOrDefault(p => p.CodigoProduto == movimentacao.CodigoProduto);

        if (produto is null)
        {
            throw new InvalidOperationException(
                $"Produto {movimentacao.CodigoProduto} nao encontrado.");
        }

        if (_movimentacoes.Any(m => m.Id == movimentacao.Id))
        {
            throw new InvalidOperationException(
                $"Ja existe uma movimentacao com o id {movimentacao.Id}.");
        }

        if (movimentacao.Tipo == TipoMovimentacao.Saida)
        {
            var saldoAtual = ConsultarSaldo(movimentacao.CodigoProduto);

            if (movimentacao.Quantidade > saldoAtual)
            {
                throw new InvalidOperationException(
                    $"Saldo insuficiente: pedido {movimentacao.Quantidade}, disponivel {saldoAtual}.");
            }
        }

        _movimentacoes.Add(movimentacao);

        return new ResultadoMovimentacao(movimentacao, ConsultarSaldo(movimentacao.CodigoProduto));
    }

    public int ConsultarSaldo(int codigoProduto)
    {
        var produto = _produtos.FirstOrDefault(p => p.CodigoProduto == codigoProduto);

        if (produto is null)
        {
            throw new InvalidOperationException($"Produto {codigoProduto} nao encontrado.");
        }

        var saldo = produto.Estoque;

        foreach (var movimentacao in _movimentacoes)
        {
            if (movimentacao.CodigoProduto != codigoProduto)
            {
                continue;
            }

            if (movimentacao.Tipo == TipoMovimentacao.Entrada)
            {
                saldo += movimentacao.Quantidade;
            }
            else
            {
                saldo -= movimentacao.Quantidade;
            }
        }

        return saldo;
    }

    public int ProximoId() => _proximoId++;
}
