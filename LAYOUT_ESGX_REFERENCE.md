# Layout do Arquivo ESGX - Saldos Gerais

## Informações Gerais

**Documento:** Arquivo de Saldos Gerais - ESGG e ESGX  
**Revisão:** 03  
**Data:** 09/10/2008  
**Utilização:** Externa  

### Tipos de Arquivo

- **ESGG**: Solicitação Automática (gerado mensalmente)
- **ESGX**: Solicitação Eventual (gerado sob demanda para data/mês específico)

---

## Estrutura do Arquivo

- **Tamanho de cada registro:** 450 bytes
- **Composição:** 5 tipos de registros

### Registros Componentes:

1. **Registro 00** - Header (Cabeçalho)
2. **Registro 01** - Identificação do Investidor
3. **Registro 02** - Lastro
4. **Registro 03** - Saldo Analítico
5. **Registro 99** - Trailer (Rodapé)

---

## 3.1 - REGISTRO 00 - HEADER

| Campo | Descrição | Tipo/Tamanho | Posição Inicial | Posição Final | Conteúdo |
|-------|-----------|--------------|-----------------|---------------|----------|
| 01 | TIPO DE REGISTRO | N(02) | 01 | 02 | FIXO "00" |
| 02 | NOME DO ARQUIVO | X(08) | 03 | 10 | FIXO "ESGG" (automático) ou "ESGX" (eventual) |
| - | CÓDIGO DO ARQUIVO | X(04) | 03 | 06 | - |
| - | CÓDIGO DO ESCRITURADOR | N(04) | 07 | 10 | Código na CBLC |
| 03 | CÓDIGO DA ORIGEM | X(08) | 11 | 18 | FIXO "CBLC" |
| 04 | CÓDIGO DO DESTINO | N(04) | 19 | 22 | Código do Escriturador |
| 05 | NÚMERO DO MOVIMENTO | N(09) | 23 | 31 | Sequencial por escriturador |
| 06 | DATA DA GERAÇÃO DO ARQUIVO | N(08) | 32 | 39 | Formato AAAAMMDD |
| 07 | DATA DO MOVIMENTO | N(08) | 40 | 47 | Formato AAAAMMDD |
| 08 | RESERVA | X(403) | 48 | 450 | Preenchido com brancos |

---

## 3.2 - REGISTRO 01 - IDENTIFICAÇÃO DO INVESTIDOR

| Campo | Descrição | Tipo/Tamanho | Posição Inicial | Posição Final | Conteúdo |
|-------|-----------|--------------|-----------------|---------------|----------|
| 01 | TIPO DE REGISTRO | N(02) | 01 | 02 | FIXO "01" |
| 02 | CPF/CNPJ DO INVESTIDOR | N(15) | 03 | 17 | - |
| 03 | DATA NASCIMENTO/FUNDAÇÃO | N(08) | 18 | 25 | Formato AAAAMMDD |
| 04 | CÓDIGO DE DEPENDÊNCIA | N(03) | 26 | 28 | - |
| 05 | NOME DO INVESTIDOR | X(60) | 29 | 88 | - |
| 06 | TIPO DE PESSOA | X(01) | 89 | 89 | F (Física) ou J (Jurídica) |
| 07 | TIPO DE INVESTIDOR | N(05) | 90 | 94 | Ver Tabela CI001 |
| 08 | NOME ADMINISTRADOR | X(13) | 95 | 107 | Nome do escriturador |
| 09 | CÓDIGO DE ATIVIDADE | N(05) | 108 | 112 | Ver Tabelas CI005 (PF) ou CI006 (PJ) |
| 10 | SEXO | X(01) | 113 | 113 | F (Feminino) ou M (Masculino) |
| 11 | ESTADO CIVIL | N(01) | 114 | 114 | Ver Tabela CI010 |
| 12 | ENDEREÇO COMPLETO | X(139) | 115 | 253 | Ver definição abaixo |
| - | - NOME DO LOGRADOURO | X(30) | 115 | 144 | - |
| - | - NÚMERO | X(05) | 145 | 149 | - |
| - | - COMPLEMENTO | X(10) | 150 | 159 | - |
| - | - BAIRRO | X(18) | 160 | 177 | - |
| - | - CIDADE | X(28) | 178 | 205 | Ver Tabela CI011 |
| - | - UF | X(02) | 206 | 207 | Sigla do Estado |
| - | - CEP | N(09) | 208 | 216 | - |
| - | - DDD/DDI TELEFONE | N(07) | 217 | 223 | - |
| - | - NÚMERO TELEFONE | N(09) | 224 | 232 | - |
| - | - RAMAL | N(05) | 233 | 237 | - |
| - | - DDD/DDI FAX | N(07) | 238 | 244 | - |
| - | - NÚMERO FAX | N(09) | 245 | 253 | - |
| 13 | DOCUMENTO DE IDENTIFICAÇÃO | X(22) | 254 | 275 | Ver definição abaixo |
| - | - NÚMERO DO DOCUMENTO | X(16) | 254 | 269 | - |
| - | - TIPO DO DOCUMENTO | X(02) | 270 | 271 | Ver Tabela CI004 |
| - | - ÓRGÃO EMISSOR | X(04) | 272 | 275 | Ver Tabela CI002 |
| 14 | SIGLA DO PAÍS DE ORIGEM | X(03) | 276 | 278 | - |
| 15 | NACIONALIDADE DO INVESTIDOR | X(15) | 279 | 293 | - |
| 16 | INFORMAÇÕES BANCÁRIAS | X(21) | 294 | 314 | Ver definição abaixo |
| - | - CÓDIGO DO BANCO | N(03) | 294 | 296 | - |
| - | - CÓDIGO DA AGÊNCIA | N(05) | 297 | 301 | - |
| - | - NÚMERO CONTA CORRENTE | X(13) | 302 | 314 | - |
| 17 | CÓDIGO ISIN | X(12) | 315 | 326 | - |
| 18 | NOME DA SOCIEDADE EMISSORA | X(12) | 327 | 338 | - |
| 19 | ESPECIFICAÇÃO | X(10) | 339 | 348 | - |
| 20 | QUANTIDADE DE ATIVOS | N(15)V03 | 349 | 366 | Com 3 casas decimais |
| 21 | DATA DE REFERÊNCIA | N(08) | 367 | 374 | Formato AAAAMMDD |
| 22 | IDENTIFICAÇÃO DO INVESTIDOR NA CBLC | N(12) | 375 | 386 | - |
| 23 | CONTROLE CBLC | X(17) | 387 | 403 | - |
| 24 | IDENTIFICAÇÃO CBLC | N(15) | 404 | 418 | - |
| 25 | TIPO DE GRAVAME | N(03) | 419 | 421 | 3 posições numéricas alinhadas com zeros à esquerda |
| 26 | RESERVA | N(01) | 422 | 422 | - |
| 27 | INDICADOR DE SALDO ANALÍTICO | X(01) | 423 | 423 | 'S' ou 'N' |
| 28 | TIPO DE ATIVO | X(03) | 424 | 426 | Ver Tabelas TAB-T e TAB-D |
| 29 | RESERVA | - | 427 | 450 | Preenchido com brancos |

---

## Observações Importantes

### Referência de Datas
- **Data de Referência (Registros 01 e 03):** Normalmente contém a data de geração do arquivo
- Se diferente da data de geração: indica que saldos estão pendentes de atualização de eventos
- A data corresponde ao último dia de negociação com direito

### Tabelas Associadas
- **CI001:** Tipo de Investidor
- **CI002:** País de Origem  
- **CI004:** Tipo de Documento
- **CI005:** Código de Atividade (Pessoa Física)
- **CI006:** Código de Atividade (Pessoa Jurídica)
- **CI007:** Data de Nascimento/Fundação
- **CI010:** Estado Civil
- **CI011:** Cidade
- **TAB-T / TAB-D:** Tipo de Gravame e Tipo de Ativo

### Controle
- Sistema numera arquivos em ordem sequencial por Escriturador
- Numeração feita no momento da gravação (campo 5 - número do movimento)

---

## Tipos de Dados

- **N(xx):** Numérico com xx dígitos
- **X(xx):** Alfanumérico com xx caracteres
- **N(xx)V03:** Numérico com xx dígitos e 3 casas decimais
