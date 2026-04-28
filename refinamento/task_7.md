# Task 7 — Orquestração do Fluxo Completo e Reconciliação

## Objetivo
Implementar o fluxo completo de processamento no `ProcessadorESGXService`, integrando leitura, validação, persistência e reconciliação, e implementar o `ReconciliarSaldosUseCase` que valida a consistência entre saldos (Tipo 02) e aquisições (Tipo 03) para uma data de movimento.

## Principais Entregas
- `ProcessadorESGXService` com fluxo completo implementado (substituindo o stub da Task 3)
- `ReconciliarSaldosUseCase` — compara soma das aquisições com o saldo total por (ISIN, CPF/CNPJ, DataMov)
- `IProcessarArquivoESGXUseCase` implementado, delegando para `ProcessadorESGXService`
- `RelatorioProcessamentoDto` populado com todos os contadores e erros ao final do processamento
- Tolerância de diferença < 0,001 na reconciliação (conforme views `VW_ESGX_RECONCILIACAO`)

## Critério de Pronto
- Dado um arquivo ESGX válido de teste, o fluxo completo executa sem erros e persiste os dados
- Registros com erro de validação (investidor ou subclasse não encontrados) são logados e ignorados
- Relatório final indica total de lidos, validados, persistidos, erros e status de reconciliação
- Registros com divergência de reconciliação são listados no relatório com detalhe

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior. Implemente o fluxo completo de processamento no ProcessadorESGXService e o ReconciliarSaldosUseCase no ms-custo-cblc.

Contexto:
O ProcessadorESGXService recebe o caminho de um arquivo ESGX, coordena leitura, validação e persistência, e retorna um RelatorioProcessamentoDto ao final.
A reconciliação valida que a soma das aquisições do Tipo 03 corresponde ao saldo do Tipo 02 para cada combinação (ISIN, CPF/CNPJ, DataMovimento), com tolerância de 0,001.

### ProcessadorESGXService — Fluxo Completo
Implemente o método ProcessarAsync(string caminhoArquivo, CancellationToken ct):

1. Gerar CorrelationId (Guid.NewGuid()) e registrar no logger com BeginScope
2. Iniciar stopwatch para medir tempo total
3. Chamar IArquivoESGXReader.LerAsync → obter ArquivoESGXLeituraDto
4. Extrair DataMovimento do Header (Registro 00)
5. Para cada RegistroTipo01Dto (investidor):
   a. Chamar IValidarVinculoInvestidorUseCase.ExecutarAsync(cpfCnpj)
   b. Se inválido: adicionar ErroValidacaoDto à lista, incrementar contador de erros, continuar
   c. Se válido: mapear para entidade Investidor com idH1 resolvido
6. Para cada RegistroTipo02Dto (saldo):
   a. Chamar IValidarVinculoSubclasseUseCase.ExecutarAsync(isin)
   b. Se inválido: adicionar erro, continuar
   c. Se válido: mapear para SaldoInvestidor
7. Para cada RegistroTipo03Dto (aquisição):
   a. Buscar investidor já validado (por CpfCnpj) e subclasse (por Isin) — usar dicionário em memória
   b. Se referências ausentes: adicionar erro, continuar
   c. Mapear para Aquisicao
8. Para cada grupo (ISIN, CPF/CNPJ, DataMov) com saldo e aquisições válidos:
   a. Iniciar transação
   b. Chamar IPersistirAquisicaoUseCase.ExecutarAsync(aquisicoes do grupo)
   c. Chamar IPersistirSaldoUseCase.ExecutarAsync(saldo do grupo)
   d. Commit ou Rollback com log de erro
9. Chamar IReconciliarSaldosUseCase.ExecutarAsync(dataMov)
10. Montar e retornar RelatorioProcessamentoDto

### ReconciliarSaldosUseCase
Implemente ReconciliarSaldosUseCase.cs (substitui stub da Task 3):
  - Recebe ISaldoInvestidorRepository e IAquisicaoRepository por injeção
  - Para cada SaldoInvestidor da DataMovimento:
    a. Listar Aquisicoes do mesmo (ISIN, CPF/CNPJ, DataMov)
    b. Somar Aquisicao.Quantidade
    c. Calcular Diferenca = Abs(SaldoQtd - SomaAquisicoes)
    d. Status = Diferenca < 0.001m ? ReconciliacaoStatus.Ok : ReconciliacaoStatus.Divergencia
  - Retornar lista de ResultadoReconciliacaoDto

Adicione ao ISaldoInvestidorRepository:
  Task<IEnumerable<SaldoInvestidor>> ListarPorDataMovimentoAsync(DataMovimento dataMov, CancellationToken ct)

### IProcessarArquivoESGXUseCase — Implementação
Implemente ProcessarArquivoESGXUseCase.cs delegando para ProcessadorESGXService:
  - Recebe ProcessadorESGXService por injeção (ou IProcessadorESGXService se preferir interface)
  - Simplesmente repassa a chamada

### Tratamento de Erros no Fluxo
- Erros de validação: registrar e continuar (nunca abortar o loop)
- Erros de persistência: rollback do grupo afetado, log com CorrelationId, continuar demais grupos
- Erros inesperados (IOException, SqlException): propagar como exceção encapsulada em ProcessamentoException com CorrelationId

### Boas Práticas
- Agrupe as aquisições por (ISIN, CPF/CNPJ) em dicionário em memória para eficiência
- Não faça consultas ao banco dentro do loop de registros — valide em lote quando possível
- Logue o início e fim de cada etapa com tempo parcial
- Nunca perca um erro silenciosamente — todo catch deve logar com nível mínimo Warning
- O RelatorioProcessamentoDto deve ser completo o suficiente para auditoria sem consultar o banco
```
