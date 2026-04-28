# Task 4 — Leitura e Parse do Arquivo ESGX

## Objetivo
Implementar na camada Infrastructure a leitura do arquivo ESGX do diretório Windows e o parser de registros fixed-width de 450 bytes — extraindo com precisão os campos de cada tipo de registro conforme o layout especificado em LAYOUT_ESGX_REFERENCE.md.

## Principais Entregas
- Interface `IArquivoESGXReader` (no Application, contrato de leitura)
- `ArquivoESGXReader` — localiza e lê o arquivo do diretório configurado
- `RegistroParser` — extrai campos por posição e tamanho (fixed-width 450 bytes)
- `ValidadorRegistro` — valida formato de tipos N, X e N(x)V03
- Mapeamento de cada tipo de registro para seu DTO correspondente
- Tratamento de encoding ASCII e conversão de tipos

## Critério de Pronto
- Dado um arquivo ESGX de teste, `ArquivoESGXReader` retorna todos os registros tipados corretamente
- Campos numéricos com decimais implícitos (V03, V08) são convertidos sem perda de precisão
- Registros 00 (Header) e 99 (Trailer) são identificados e processados
- Tipos de registro desconhecidos são logados e ignorados sem abortar o processamento

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior. Implemente o parser do arquivo ESGX no projeto MsCustoCblc.Infrastructure.

Contexto:
O arquivo ESGX é um arquivo texto de largura fixa com 450 bytes por registro (linha).
Cada registro começa com 2 bytes que identificam o tipo: "00" (Header), "01" (Investidor), "02" (Saldo/Lastro), "03" (Saldo Analítico/Aquisição), "99" (Trailer).
Os campos são extraídos por posição de início e fim (1-indexed).
Tipos de dados: N(xx) = numérico, X(xx) = alfanumérico, N(xx)V03 = numérico com 3 casas decimais implícitas (sem ponto), N(xx)V08 = 8 casas decimais implícitas.

### Interface de Leitura
Crie em MsCustoCblc.Application/UseCases/IArquivoESGXReader.cs:
  - Task<ArquivoESGXLeituraDto> LerAsync(string caminhoArquivo, CancellationToken ct)

Crie ArquivoESGXLeituraDto com:
  - Header (RegistroHeaderDto)
  - RegistrosTipo01 (IReadOnlyList<RegistroTipo01Dto>)
  - RegistrosTipo02 (IReadOnlyList<RegistroTipo02Dto>)
  - RegistrosTipo03 (IReadOnlyList<RegistroTipo03Dto>)
  - TotalLinhas (int)
  - LinhasIgnoradas (int)

### ArquivoESGXReader
Crie em MsCustoCblc.Infrastructure/FileProcessing/ArquivoESGXReader.cs:
  - Implementa IArquivoESGXReader
  - Recebe IOptions<FileSettings> e ILogger<ArquivoESGXReader> por injeção
  - Valida existência do arquivo antes de ler
  - Lê o arquivo linha a linha com StreamReader (encoding Latin1/ISO-8859-1 para compatibilidade com ASCII legado)
  - Identifica o tipo de registro pelos 2 primeiros bytes de cada linha
  - Delega o parse de cada linha ao RegistroParser correspondente
  - Loga WARNING para linhas com tamanho diferente de 450 bytes
  - Loga WARNING para tipos de registro desconhecidos, sem abortar

### RegistroParser
Crie em MsCustoCblc.Infrastructure/FileProcessing/RegistroParser.cs:
  - Classe estática com métodos para cada tipo de registro
  - Método auxiliar privado: string ExtrairCampo(string linha, int inicio, int fim) — extrai substring (posições 1-indexed, inclusive), faz Trim()
  - Método auxiliar privado: decimal ParseDecimalImplicito(string valor, int casasDecimais) — converte "000001234567" com 8 casas em 0.01234567
  - Método auxiliar privado: DateOnly? ParseData(string valor) — converte "AAAAMMDD" em DateOnly, retorna null se inválido

  ParseHeader(string linha) → RegistroHeaderDto:
    - Tipo (1-2), NomeArquivo (3-10), CodigoOrigem (11-18), CodigoDestino (19-22), NumeroMovimento (23-31), DataGeracao (32-39), DataMovimento (40-47)

  ParseTipo01(string linha) → RegistroTipo01Dto:
    - Seguir exatamente as posições do LAYOUT_ESGX_REFERENCE.md:
      CpfCnpj (3-17), DataNascFundacao (18-25), CodDependencia (26-28), NomeInvestidor (29-88),
      TipoPessoa (89-89), TipoInvestidor (90-94), NomeAdministrador (95-107), CodigoAtividade (108-112),
      Sexo (113-113), EstadoCivil (114-114), Logradouro (115-144), Numero (145-149),
      Complemento (150-159), Bairro (160-177), Cidade (178-205), UF (206-207), Cep (208-216),
      DddTelefone (217-223), NumeroTelefone (224-232), Ramal (233-237),
      DddFax (238-244), NumeroFax (245-253), NumeroDocumento (254-269), TipoDocumento (270-271),
      OrgaoEmissor (272-275), SiglaPais (276-278), Nacionalidade (279-293),
      CodigoBanco (294-296), CodigoAgencia (297-301), ContaCorrente (302-314),
      Isin (315-326), NomeEmissora (327-338), Especificacao (339-348),
      QuantidadeAtivos (349-366 → N15V03 = divide por 1000),
      DataReferencia (367-374), IdCblcInvestidor (375-386), ControleCblc (387-403),
      IdentificacaoCblc (404-418), TipoGravame (419-421), Reserva422 (422-422),
      IndicadorSaldoAnalitico (423-423), TipoAtivo (424-426)

  ParseTipo02(string linha) → RegistroTipo02Dto:
    - Layout do Registro 02 não está completamente documentado em LAYOUT_ESGX_REFERENCE.md
    - Extraia os campos disponíveis e adicione comentário TODO indicando que o layout precisa ser confirmado
    - Por ora extraia: TipoRegistro (1-2) e deixe os demais campos como strings brutas para mapeamento futuro

  ParseTipo03(string linha) → RegistroTipo03Dto:
    - Similar ao Tipo 02 — extraia TipoRegistro e campos equivalentes a: CpfCnpj, Isin, DataAquisicao, PrecoUnitario, Quantidade
    - Adicione comentário TODO para confirmação do layout completo

### ValidadorRegistro
Crie em MsCustoCblc.Infrastructure/FileProcessing/ValidadorRegistro.cs:
  - ValidarTamanhoLinha(string linha) → bool (deve ter 450 chars)
  - ValidarCampoNumerico(string valor, string nomeCampo) → (bool Valido, string? Erro)
  - ValidarCampoData(string valor, string nomeCampo) → (bool Valido, string? Erro)
  - ValidarCpfCnpj(string valor) → (bool Valido, string? Erro) — valida tamanho (11 ou 14-15)
  - ValidarIsin(string valor) → (bool Valido, string? Erro) — valida 12 chars alfanumérico

### Configurações
Crie FileSettings.cs com propriedades EsgxDirectory (string), EsgxFilePattern (string).
Registre via IOptions<FileSettings> no DI.

### Registro de Dependências
Adicione ao método AddInfrastructureServices() (criar se não existir):
  services.AddScoped<IArquivoESGXReader, ArquivoESGXReader>()
  services.Configure<FileSettings>(configuration.GetSection("FileSettings"))

### Boas Práticas
- Use StreamReader com using para garantir fechamento do arquivo
- Prefira ReadLineAsync para não bloquear a thread
- Nunca lance exceção para linha com formato inválido — registre o erro e continue
- Não misture lógica de negócio com leitura de arquivo
- Todos os métodos de parse devem ser puros (sem estado, sem I/O)
```
