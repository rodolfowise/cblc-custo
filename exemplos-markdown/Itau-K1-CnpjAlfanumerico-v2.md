# Itaú Unibanco - Adaptação do Sistema K1 ao CNPJ Alfanumérico

**Versão:** v2  •  **Data:** 18.07.2025  •  **Páginas:** 11

---

## Página 1

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* INTRODUÇÃO
*  referências legais
*  considerações sobre o ambiente
*  considerações sobre o cronograma
*  especificações fundamentais
*  escopo e prazo de cada entrega
*  custo em horas do projeto
* versões do documento
* eventos de levantamento
* Wise House Engenharia de Sistemas 1


---

## Página 2

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* REFERÊNCIAS LEGAIS
*  Anexo XV da Instrução Normativa RFB nº 2.119, de 6 de dezembro de 2022
* Wise House Engenharia de Sistemas 2


### Tabela 1

| Parágrafo único. O CNPJ adotará o formato alfanumérico composto por quatorze posições, conforme disposto no Anexo XV, com previsão |
|---|
| de implementação a partir de julho de 2026. |


---

## Página 3

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* CONSIDERAÇÕES SOBRE O AMBIENTE
* Wise House Engenharia de Sistemas 3


---

## Página 4

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* CONSIDERAÇÕES SOBRE O CRONOGRAMA
* Wise House Engenharia de Sistemas 4


---

## Página 5

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* ESPECIFICAÇÕES FUNDAMENTAIS
* Wise House Engenharia de Sistemas 5


---

## Página 6

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

V2 18.07.2025
7.

Quanto aos layouts das interfaces de entrada recebidas pelo K1, considera-se que elas não sofrerão alteração
no tamanho e posicao da informacao CNPJ, quando trazem essa informação.

Considera-se que nenhuma delas
fixe tamanho menor que 14 dígitos para essa informação.

 Em tempo: todas as atuais interfaces de entrada com layout posicional que trazem CPF CNPJ reservam
ao menos 14 dígitos para esse dado.

8.

Idem quanto aos layouts das interfaces de saída entregues pelo K1.

As interfaces abaixo são exemplos aos quais se aplicam essa condição:
 Conteúdo XML (layout XDDOC) enviado ao XD/ER6 para pagamento a investidor.

Esse conteúdo contém a informação CNPJCPF em diversos pontos, sob tags nomeadas como
“CNPJ_CPF....”.

Essas tags estão definidas como numéricas, embora aceitem 14 dígitos.

Considera-se
que os CNPJs alfanuméricos com letras poderão ser enviados nessas tags e que serão aceitos pelo
XD/ER6.

 Arquivo K1DDC enviado ao sistema CC para pedido de criação ou atualização de conta RUC.

O layout contém o campo ‘CGCCPF’, alfanumérico de tamanho 15, já adequado portanto para receber
os conteúdos alfanuméricos e complementados por zeros que o H1 enviará.

Considera-se que o CC
acolherá as letras e os zeros à esquerda.

9.

Quanto às Interfaces de entrada ou saída que sejam implementadas através de funções ou procedures
executadas diretamente pelos sistemas ou processos fornecedores/consumidores:
Nesses casos, os conteúdos são recebidos/entregues na forma de tabelas ou de parâmetros dessas funções ou
procedures.

Nos casos em que CNPJ/CPF estejam com datatype numérico, eles serão alterados para datatype
Varchar.

As procedures ou funções que no momento apresentam alguma condição nesse sentido são:
 SPK1_EU8_PGTO, executada pelo EU8 para marcar no banco do K1 os pagamentos que ele próprio
tenha realizado.

Essa procedure tem CNPJCPF como parm de entrada, definido como Numeric, e será alterada para
definir e tratar esse parm como Varchar.

Considera-se que o EU8 ajustará a chamada dessa procedure
para tratar esse parametro como Varchar.

 Conteúdo enviado ao LK via Webservice, para pagamento à Bolsa e à Cetip via mensageria.

Esse conteúdo contém a raiz do CNPJ do banco liquidante e da bolsa ou Cetip.

Considera-se que o LK
admitirá letras nesse conteúdo.

 Procedures SPK1_EU8_EANL2 e SPK1_EU8_ESMA2, executadas pelo EU8 para envio de pagamentos e
saldos ao DF.

Os conteúdos de saída dessas procedures contêm a informação CNPJ CPF, num único campo.

Em ambas
as procedures esse campo já está com formato Varchar(14), não demandando portanto nenhuma
alteração por parte do K1.

Considera-se apenas que o EU8 esteja ajustado à presença de letras nesse
campo ao tratar o conteúdo entregue pelas procedures.

10.

Quanto às telas de cadastro em que figurem CPF e CNPJ (por exemplo cadastro de investidor, cadastro de
empresa emissora e cadastro da Ficha 617), elas terão provisoriamente uma trava parametrizada no webconfig
que impedirá que se cadastrem CNPJs e CPFs com letras, embora não impeça a apresentação de CNPJs ou CPFs
com letras que por qualquer outra razão (por exemplo carga de arquivo de retirada bolsa) já estejam nas
respectivas tabelas.

Essa trava evitará que a área operacional cadastre acidentalmente letras nesses campos
enquanto o sistema não estiver integralmente adaptado.

Esse parâmetro ficará ativado a partir da entrega
desses cadastros adaptados, e será desativado no pacote da última entrega.

Os testes da entrega que contém
esses cadastros deverão ser planejados de forma que se testem esses cadastros tanto com o parâmetro ativado
como desativado.

Wise House Engenharia de Sistemas 6


---

## Página 7

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

V2 18.07.2025
11.

Em relação às letras que compõem um CNPJ alfanumérico, elas serão gravadas sempre em maiúsculas no banco
de dados, mesmo que tenham sido informadas em minúsculas no ato do cadastro.

Tal definição também se
estende às interfaces de entrada de dados recebidas pelo K1.

Wise House Engenharia de Sistemas 7


---

## Página 8

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* ESCOPO – CONSIDERAÇÕES INICIAIS
* Wise House Engenharia de Sistemas 8


---

## Página 9

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* ESCOPO E PRAZO DA 1ª ENTREGA
* Conversão integral do banco de dados
* ----
* ---
* Função de cálculo do DAC do CNPJ
* Cadastro de investidores
* Cadastro de Ficha 617, com sincronização bi-direcional
* Integração LIQD2 (depósitos e retiradas CBLC)
* Aviso de alteração cadastral
* Funções da integração Conta RUC
* Carga dos arquivos CBLC ESGM e ESGX
* Relatórios de Investidor e Ativo CBLC
* Carga dos arquivos CETIP DSTOTAIS e DSMODIF
* Relatórios de Investidor e Ativo CETIP
* Conciliação de Lastros CBLC e CETIP
* Carga do arquivo de Gravames CETIP
* Relatório Gravame Analítico
* Cálculo de pagamento DEB e MNP (alterações nos filtros)
* Escritural/Liberação, Escritural/Envio, vRetornos, Retornos por data
* ----
* cenários de letra no CNPJ
* ----
* Wise House Engenharia de Sistemas 9


---

## Página 10

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

* V2 18.07.2025
* ESCOPO E PRAZO DA 2ª ENTREGA
* durante os testes.)
* ---
* Movimentações DEB e MNP (alterações nos filtros)
* Função de zeragem de saldos Bolsa
* Envio LK
* Avisos (Pagamento, Movimentos, Extrato de posição)
* Faturamento/Integração OK2
* Relatórios
* Interfaces de saida
* Interface CCS
* Livro escritural
* Informes DF (procedures acionadas pelo EU8)
* Módulo de custódia física
* Recompra
* Conversão
* Teste do rollback, removendo os cenários de letra no CNPJ
* Wise House Engenharia de Sistemas 10


---

## Página 11

### ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO

V2 18.07.2025

### CUSTO EM HORAS

* o Desenvolvimento
* o Teste de fornecedor
* o Apoio à homologação do Itaú
* o Apoio à validação pós-implantação
* (conforme apresentado nas páginas de Escopo)
* (conforme apresentado nas páginas de Escopo)
* Adaptação do K1 ao CNPJ alfanumérico custo (horas)
* Wise House Engenharia de Sistemas 11


### Tabela 1

| Adaptação do K1 ao CNPJ alfanumérico | custo (horas) |
|---|---|
| — | 2.560 |


---

