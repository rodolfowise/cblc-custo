# ITAÚ UNIBANCO - ADAPTAÇÃO DO SISTEMA K1 AO CNPJ ALFANUMÉRICO
## ESCOPO — V2 18.07.2025
*Wise House Engenharia de Sistemas*

---

## INTRODUÇÃO

A Receita Federal emitiu em maio/2024 a *Nota Técnica conjunta COCAD/SUARA/RFB nº 49 de 14/5/24*, adotando o CNPJ alfanumérico como alternativa para sua expansão. As partes *RAIZ* e *ORDEM* (4 dígitos após a raiz) do CNPJ passarão a conter também letras, e os dois dígitos de controle continuarão numéricos. E o tamanho total do CNPJ continuará sendo de 14 dígitos.

> O novo cálculo do dígito de controle foi definido de forma a preservar os valores que atualmente são gerados para os conteúdos exclusivamente numéricos.

A referida Nota Técnica definiu janeiro/2026 como início da emissão dos CNPJ's com letras, data que foi adiada para **julho/2026** pela Instrução Normativa RFB nº 2229, de 15/10/24.

Nesse documento a Wise House apresenta proposta ao Itaú Unibanco para a adaptação do sistema K1 ao CNPJ alfanumérico, decompondo a proposta nos seguintes compromissos e considerações:

- referências legais
- considerações sobre o ambiente
- considerações sobre o cronograma
- especificações fundamentais
- escopo e prazo de cada entrega
- custo em horas do projeto

---

### Versões do documento

- v1 01.07.2025 - Wise House - Elaboração original.
- V2 18.07.2025 - Wise House - Ajustes na composição dos pacotes.
- V3 23.04.2026 - Wise House - Conversao do doc para .md.
- V4 23.04.2026 - Wise House - Alteracao da estimativa de horas

---

### Eventos de levantamento

- Conversação por email entre Itaú (Marega) e Wise House de 14/08/24 a 18/10/24, sob título 'Campo CNPJ alfanumérico', avaliando superficialmente o impacto do CNPJ alfanumérico no sistema K1, conforme definido pela *Nota Técnica conjunta COCAD/SUARA/RFB nº 49 de 14 de maio de 2024*.

- Estimativa e escopo preliminares da adaptação do K1 enviada pela Wise House em email de 18/3/25 sob título 'K1 CNPJ Alfanumerico - estimativa 14/3'.

- Email do Itaú (Marega) em 11/4/25 sob título 'Orçamento CNPJ Alfanumérico' solicitando à Wise House estimativa de prazo para adaptação do K1.

- Call em 17/4/25 com Itaú (Marega e Lucas) para discussão sobre as alternativas de fracionamento das entregas.

- Estimativa final de escopo, de esforço e de prazo enviada ao Itaú pela Wise em email de 23/4/25, sob título 'Orçamento CNPJ Alfanumérico', considerando uma das alternativas de faseamento com entrega em duas partes, ampliando os esforços de teste da estimativa preliminar e agregando esforços de apoio à homologação.

- Email do Itaú (Marega) em 9/5/25 sob título 'Orçamento CNPJ Alfanumérico', apresentando preferência pelo faseamento de entregas que se inicia pela conversão do banco de dados e perguntando à Wise House sobre sua disponibilidade para realização desse projeto de adaptação do K1.

- Call em 30/6/25 com Itaú (Marega e Lucas) em que Marega comunicou o interesse do Itaú em fechar a contratação desse projeto.

---

## REFERÊNCIAS LEGAIS

- Nota Técnica conjunta COCAD/SUARA/RFB nº 49 de 14 de maio de 2024.

- Instrução Normativa RFB nº 2119, de 6 de dezembro de 2022, no Art. 2º do CAPÍTULO I, Parágrafo único (incluído(a) pelo(a) Instrução Normativa RFB nº 2229, de 15 de outubro de 2024):

  > *Parágrafo único. O CNPJ adotará o formato alfanumérico composto por quatorze posições, conforme disposto no Anexo XV, com previsão de implementação a partir de julho de 2026.*

- Anexo XV da Instrução Normativa RFB nº 2.119, de 6 de dezembro de 2022 (incluído(a) pelo(a) Instrução Normativa RFB nº 2229, de 15 de outubro de 2024).

### Informações Básicas: CNPJ Numérico x CNPJ Alfanumérico

| | CNPJ numérico | CNPJ alfanumérico |
|---|---|---|
| Números existentes | Serão mantidos | Será destinado a novas inscrições |
| Tamanho | 14 posições | 14 posições |
| 1ª a 8ª posições | Numéricas, compondo a raiz do CNPJ | Alfanuméricas, compondo a raiz do CNPJ |
| 9ª a 12ª posições | Numéricas, identificando a ordem do estabelecimento | Alfanuméricas, identificando a ordem do estabelecimento |
| 13ª e 14ª posições | Numéricas, identificando os dígitos verificadores | Numéricas, identificando os dígitos verificadores |

### Composição: CNPJ Numérico x CNPJ Alfanumérico

| CNPJ (14 posições) | CNPJ alfanumérico (14 posições) |
|---|---|
| NN.NNN.NNN / NNNN - NN | SS.SSS.SSS / SSSS - NN |
| RAIZ / ORDEM / DV | RAIZ / ORDEM / DV |
| N = Número | N = Número / S = Letra e Número |

### Cálculo do Dígito Verificador

| CNPJ numérico | CNPJ alfanumérico |
|---|---|
| Cálculo pelo Módulo 11 | Cálculo pelo Módulo 11 |

### Atribuição de valores aos números e às letras do CNPJ

- **4.1.1** Os valores decimais, contidos na Tabela Código ASCII, serão atribuídos aos valores numéricos e alfanuméricos do novo CNPJ.
- **4.1.2** Valores numéricos serão substituídos pelo valor decimal constante da tabela código ASCII e, para cada um deles, subtraído o valor 48.
- **4.1.3** Valores alfanuméricos serão substituídos pelos valores decimais relativos às letras maiúsculas da tabela código ASCII e, para cada um deles, subtraído o valor 48.
- **4.1.4** Dessa forma, obtêm-se os valores para cada atributo do novo CNPJ.

---

## CONSIDERAÇÕES SOBRE O AMBIENTE

1. Considera-se que o desenvolvimento será realizado no ambiente da Wise House, decorrendo disso que:
   - Os fontes (C# e stored-procedures) em alteração estarão no ambiente da Wise House, sejam os fontes já presentes na Wise House, sejam fontes que o Itaú envie para a Wise House antes do início das alterações.
   - As definições de banco de dados (tabelas, campos, views) que sofrerão as adaptações serão as definições atualmente presentes no ambiente da Wise House, atualizadas pelas modificações feitas pelo Itaú nos últimos meses que o Itaú comunique à Wise House antes do início das alterações.
   - Os pacotes entregues pela Wise House serão compostos por fontes C#, fontes de stored-procedures, scripts de alteração de tabelas/campos/views, e scripts de alteração de dados.
   - Considera-se que o Itaú, ao receber os pacotes da Wise House, os conciliará com as versões presentes no seu ambiente, com o intuito estrito de revelar eventuais alterações que já tenha realizado e não estejam presentes nos fontes enviados pela Wise House.

2. Considera-se que o teste de desenvolvedor será realizado no ambiente da Wise House, decorrendo disso que:
   - Os cenários e as massas de teste utilizados não necessariamente serão reprodutíveis no ambiente do Itaú.
   - Os prazos e agenda do Itaú para sua própria homologação desse projeto estão fora do domínio da Wise House, sem prejuízo do apoio da Wise House a essa homologação, apoio esse que está considerado nessa proposta.
   - Os testes das integrações realizados na Wise House baseiam-se em arquivos e recursos que emulam parcialmente as integrações no Itaú, e restringem-se à validação do tratamento ou geração de conteúdos. Somente na configuração do Itaú esses testes poderão ser plenamente realizados.
   - Da mesma forma, os testes de performance realizados no ambiente da Wise House são insuficientes para confirmar o comportamento no Itaú quanto a esse quesito.

---

## CONSIDERAÇÕES SOBRE O CRONOGRAMA

1. A conversão do banco de dados (atividade descrita mais à frente, nos detalhes de especificação) deve ser integralmente aplicada numa única execução, independentemente de qual seja o fracionamento de entregas que se adote. Não importando no entanto se esse tombamento ocorra já na primeira entrega ou seja protelado para entregas finais, variando somente o momento em que ocorrerá o esforço de teste regressivo exigido pela conversão do banco.
   - *Em tempo: Essa proposta considera a conversão do banco já na primeira entrega.*

2. O fracionamento de entregas, embora exija mais de uma mobilização da área operacional para a homologação, permite paralelismo entre a atividade de desenvolvimento da Wise House e a atividade do Itaú na validação e implantação. O acompanhamento da Wise House à homologação de uma entrega pelo Itaú ocorre ao mesmo tempo em que se inicia o desenvolvimento da entrega seguinte. E os roteiros de teste e as recomendações de massas de teste, das entregas subsequentes à primeira, podem ser elaborados e entregues antecipadamente ao Itaú.
   - *Em tempo: Essa proposta considera duas entregas.*

3. Consideramos que o surgimento de CNPJs alfanuméricos no contexto do K1 poderá ocorrer assim que a Receita Federal comece emiti-los, por exemplo por uma empresa recém criada que traga ativos para a escrituração Itaú, ou por um fundo novo (ou uma 'Classe' nova de um fundo já existente e em conformidade com a ICVM-175) que compre na Cetip ativos escriturados no K1.


4. Em tempo: ressaltamos a criticidade de cumprimento dos prazos desse projeto por conta dos arquivos da CBLC (ESGM/X, EDIV/X) que já foram adaptados pela B3, e estarão em produção em maio/2026 nessa versão nova, com desativação das versões anteriores..
---

## ESPECIFICAÇÕES FUNDAMENTAIS

1. Dado que CNPJ e CPF sempre participarão como dados alfanuméricos, o K1 fará sempre o completo preenchimento de seus dígitos com zeros à esquerda, seja ao armazená-los no banco, seja nos processos e pontos de lógica SQL em que participam, seja nos relatórios e nas interfaces de saída.

2. Nos pontos em que o tipo de pessoa (PF/PJ) do detentor do CPF CNPJ é obrigatoriamente conhecido, o K1 completará o CNPJ com 14 dígitos e o CPF com 11 dígitos. Esses pontos são:
   - Cálculo do DAC do CPF CNPJ.
   - Consulta ao serviço SERPRO de situação do CPF CNPJ.
   - Cadastro do investidor, em que CNPJ e CPF estão em campos separados.
   - Outras tabelas em que CNPJ e CPF estejam em campos separados.
   - Em tabelas em que CPF e CNPJ estão fundidos num único campo, quando o detentor do CPF CNPJ tem seu tipo de pessoa obrigatoriamente definido.
   - Em saídas, relatórios e telas, quando o detentor tem seu tipo de pessoa obrigatoriamente definido.
     - *Exceção: essa explícita complementação em saídas relatórios e telas ocorrerá somente nos casos em que o CPF CNPJ esteja apresentado com formatação, sendo que esse escopo não considera a aplicação de formatação onde ela já não exista atualmente. Nesses casos o CPF CNPJ será apresentado como a fonte da informação o estiver entregando, que na maioria dos casos estará consistente com a definição de 14 e 11 dígitos acima referida.*

3. Não está previsto desenvolvimento ou adoção de função que distinga um CNPJ de um CPF pela análise de seus dígitos.

4. Como decorrência da restrição acima, em todos os casos em que não é explícita a distinção do dado como sendo um CPF ou CNPJ, como por exemplo em arquivos da Cetip, o K1 completará o dado com zeros à esquerda completando 15 dígitos.

   Quinze dígitos têm o mérito de não privilegiar nem o CNPJ nem o CPF, evitando ambiguidade. E têm também o mérito de que são um tamanho largamente adotado até aqui por todos os layouts e datatypes que acolhem CNPJ e CPF como alfanuméricos, mesmo quando não consideram complementação com zeros à esquerda.

5. Nos algoritmos e lógicas SQL em que CPF CNPJ participem como chave (em *order by*, *group by*, *having*, clausulas de *join*, condições de *case*, controles de fluxo, *distinct*, *row_number*, *partition*, *min*, *max*), eles serão completados para 15 dígitos quando houver incidência de casos com tipo de pessoa desconhecida. Nos casos em que as fontes dos dados sejam todas fiéis à definição de 14 e 11 dígitos de CNPJ e CPF, nesses casos não haverá complementação, e os CPF e CNPJ participarão das referidas lógicas com seus exatos caracteres.

6. A conversão do banco de dados alterará para *Varchar*(15) todos os campos de CPF e CNPJ que já não estejam como *Varchar*, e alterará seus conteúdos complementando-os com zeros à esquerda. Em coerência com o definido até aqui, completará os CPFs com 11 dígitos e os CNPJs com 14 dígitos, onde forem campos separados ou estiverem obrigatoriamente distintos pelo tipo de pessoa do detentor. E com 15 dígitos, onde não houver condição de distinção assegurada.
   - *Nota: Quanto a tabelas que contêm campos em que seu conteúdo seja a concatenação de CPFs ou CNPJs com outras informações, considera-se que nessas concatenações eles já tenham sido complementados com zeros à esquerda de forma a preencherem todos os dígitos que o layout do campo lhes reserva. Essas tabelas não sofrerão conversão.*

7. Quanto aos layouts das interfaces de entrada recebidas pelo K1, considera-se que elas não sofrerão alteração no tamanho e posição da informação CNPJ, quando trazem essa informação. Considera-se que nenhuma delas fixe tamanho menor que 14 dígitos para essa informação.
   - *Em tempo: todas as atuais interfaces de entrada com layout posicional que trazem CPF CNPJ reservam ao menos 14 dígitos para esse dado.*

8. Idem quanto aos layouts das interfaces de saída entregues pelo K1. As interfaces abaixo são exemplos aos quais se aplicam essa condição:
   - **Conteúdo XML (layout XDDOC)** enviado ao XD/ER6 para pagamento a investidor. Esse conteúdo contém a informação CNPJCPF em diversos pontos, sob tags nomeadas como "CNPJ_CPF....". Essas tags estão definidas como numéricas, embora aceitem 14 dígitos. Considera-se que os CNPJs alfanuméricos com letras poderão ser enviados nessas tags e que serão aceitos pelo XD/ER6.
   - **Arquivo K1DDC** enviado ao sistema CC para pedido de criação ou atualização de conta RUC. O layout contém o campo 'CGCCPF', alfanumérico de tamanho 15, já adequado portanto para receber os conteúdos alfanuméricos e complementados por zeros que o H1 enviará. Considera-se que o CC acolherá as letras e os zeros à esquerda.

9. Quanto às Interfaces de entrada ou saída que sejam implementadas através de funções ou procedures executadas diretamente pelos sistemas ou processos fornecedores/consumidores: Nesses casos, os conteúdos são recebidos/entregues na forma de tabelas ou de parâmetros dessas funções ou procedures. Nos casos em que CNPJ/CPF estejam com datatype numérico, eles serão alterados para datatype *Varchar*.

   As procedures ou funções que no momento apresentam alguma condição nesse sentido são:
   - **SPK1_EU8_PGTO**, executada pelo EU8 para marcar no banco do K1 os pagamentos que ele próprio tenha realizado. Essa procedure tem CNPJCPF como parm de entrada, definido como Numeric, e será alterada para definir e tratar esse parm como Varchar. **Considera-se que o EU8 ajustará a chamada dessa procedure para tratar esse parametro como Varchar.**
   - **Conteúdo enviado ao LK via Webservice**, para pagamento à Bolsa e à Cetip via mensageria. Esse conteúdo contém a raiz do CNPJ do banco liquidante e da bolsa ou Cetip. **Considera-se que o LK admitirá letras nesse conteúdo.**
   - **Procedures SPK1_EU8_EANL2 e SPK1_EU8_ESMA2**, executadas pelo EU8 para envio de pagamentos e saldos ao DF. Os conteúdos de saída dessas procedures contêm a informação CNPJ CPF, num único campo. Em ambas as procedures esse campo já está com formato Varchar(14), não demandando portanto nenhuma alteração por parte do K1. **Considera-se apenas que o EU8 esteja ajustado à presença de letras nesse campo ao tratar o conteúdo entregue pelas procedures.**

10. Quanto às telas de cadastro em que figurem CPF e CNPJ (por exemplo cadastro de investidor, cadastro de empresa emissora e cadastro da Ficha 617), elas terão provisoriamente uma trava parametrizada no webconfig que impedirá que se cadastrem CNPJs e CPFs com letras, embora não impeça a apresentação de CNPJs ou CPFs com letras que por qualquer outra razão (por exemplo carga de arquivo de retirada bolsa) já estejam nas respectivas tabelas. Essa trava evitará que a área operacional cadastre acidentalmente letras nesses campos enquanto o sistema não estiver integralmente adaptado. Esse parâmetro ficará ativado a partir da entrega desses cadastros adaptados, e será desativado no pacote da última entrega. Os testes da entrega que contém esses cadastros deverão ser planejados de forma que se testem esses cadastros tanto com o parâmetro ativado como desativado.

11. Em relação às letras que compõem um CNPJ alfanumérico, elas serão gravadas sempre em maiúsculas no banco de dados, mesmo que tenham sido informadas em minúsculas no ato do cadastro. Tal definição também se estende às interfaces de entrada de dados recebidas pelo K1.

---

## ESCOPO – CONSIDERAÇÕES INICIAIS

Essa proposta compreende a adaptação de todas as tabelas, campos, views, funções C# e stored-procedures do K1 que contêm, recebem, tratam e entregam CPF e CNPJ, para que reconheçam conteúdo alfanumérico dessas informações, e preservem integralmente sua atual funcionalidade.

Essa adaptação considera as *Especificações fundamentais* apresentadas no tópico anterior.

O escopo está apresentado na forma de lista de processos e funções impactados pela adaptação, seja porque sofrerão alterações diretamente, seja porque envolvem CPF CNPJ embora sem necessidade de adaptação, todos no entanto requerendo validação.

Os entregáveis são arquivos separados com as seguintes finalidades:

1. Script de conversão do banco, alterando datatypes dos campos de CPF e/ou CNPJ
2. Script de ajuste dos valores de CPF CNPJ, alterando o conteúdo desses campos, complementando-os com zeros
3. Fontes C# alterados
4. Scripts SQL de alteração de procedures, funções e views
5. Arquivos de rollback da conversão do banco
6. Arquivos de rollback dos fontes C#
7. Arquivos de rollback dos scripts SQL
8. Roteiros de teste, na forma de listagem via planilha excel das funções que devem ser testadas, apresentando sucintamente para cada uma a sua rota, o resultado esperado, e uma indicação do cenário a ser testado.

**Nota sobre rollback:** O rollback da conversão do banco de dados será basicamente o retorno para *Numeric* dos campos que tenham sido alterados para *Varchar*. Dessa forma todas as complementações com zero que tenham sido realizadas pelo script de conversão serão **automaticamente** revertidas para os valores numéricos originais. Considera-se que nenhuma letra tenha sido inserida em qualquer desses campos entre a implantação e o momento do rollback, mesmo porque as telas de cadastro que sejam entregues junto com a conversão do banco estarão com bloqueio de entrada de letra, por conta do parâmetro do webconfig de bloqueio descrito no tópico *Especificações fundamentais*, que estará ativado em produção.

Essa proposta considera duas entregas, cujos escopos e prazos são apresentados nas próximas páginas.

---

## ESCOPO E PRAZO DA 1ª ENTREGA

### Composição

- Conversão integral do banco de dados
- Entrega do webconfig ativando o flag de bloqueio de entrada de letra nos cadastros de CPF_CNPJ *(Nota: considera-se que durante os testes seja possível ativar e desativar esse flag.)*
- Função de cálculo do DAC do CNPJ
- Serviço 'Serpro' de consulta à situação do CPF/CNPJ (teste possível somente no ambiente Itaú)
- Cadastro de empresas, contratos e papéis, e respectiva tela de Auditoria cadastral ('Auditoria Sintética' e 'Analítica')
- Cadastro de investidores
- Cadastro de Ficha 617, com sincronização bi-direcional
- Integração LIQD2 (depósitos e retiradas CBLC)
- Aviso de alteração cadastral
- Funções da integração Conta RUC
- Fechamento manual do sistema (ênfase nas chamadas de RUC e nos avisos gerados pelo sistema)
- Robô de fechamento/abertura (K1M0101) (ênfase nas chamadas de RUC e de faturamento)
- Carga dos arquivos CBLC ESGM e ESGX
- Relatórios de Investidor e Ativo CBLC
- Carga dos arquivos CETIP DSTOTAIS e DSMODIF
- Relatórios de Investidor e Ativo CETIP
- Conciliação de Lastros CBLC e CETIP
- Carga do arquivo de Gravames CETIP
- Relatório Gravame Analítico
- Cálculo de pagamento DEB e MNP (alterações nos filtros)
- Telas de liquidação DEB e MNP: Escritural/Liberação, Escritural/Envio, vRetornos, Retornos por data

### Plano básico de testes

- Teste regressivo do sistema, SEM cenário de letra no CNPJ (por conta da conversão do banco)
- Teste acentuado das funções do pacote, SEM cenário de letra no CNPJ e flag de bloqueio ativado
- Teste acentuado das funções do pacote, COM cenário de letra no CNPJ e flag de bloqueio desativado
- Teste do rollback das funções (que também é um teste regressivo do sistema) e procedures, removendo os cenários de letra no CNPJ

### Entregáveis

1. Script de conversão dos datatypes para *Varchar*
2. Script de complementação com zero dos campos alterados para Varchar
3. Diagrama de arquitetura com as camadas da aplicação
4. Roteiro do teste regressivo
5. Rollback da conversão de datatypes
6. Fontes C# alterados
7. Scripts SQL de alteração de procedures, funções e views
8. Rollback dos fontes C#
9. Rollback dos scripts SQL
10. Roteiro do teste das funções e procedures alteradas

### Prazo de entrega

**3 meses.**

---

## ESCOPO E PRAZO DA 2ª ENTREGA

### Composição

- Entrega do webconfig desativando o flag de bloqueio de entrada de letra nos cadastros de CPF_CNPJ *(Nota: dado que essa será a entrega final, esse flag ficará definitivamente desativado, não sendo necessário alterá-lo durante os testes.)*
- Movimentações DEB e MNP (alterações nos filtros)
- Função de zeragem de saldos Bolsa
- Envio LK
- Avisos (Pagamento, Movimentos, Extrato de posição)
- Faturamento/Integração OK2
- Relatórios
- Interfaces de saída
- Interface CCS
- Livro escritural
- Informes DF (procedures acionadas pelo EU8)
- Módulo de custódia física
- Recompra
- Conversão

### Plano básico de testes

- Teste das funções do pacote, COM cenário de letra no CNPJ e flag de bloqueio desativado
- Teste do rollback, **removendo os cenários de letra no CNPJ**

### Entregáveis

1. Fontes C# alterados
2. Scripts SQL de alteração de procedures, funções e views
3. Arquivos de rollback dos fontes C#
4. Arquivos de rollback dos scripts SQL
5. Roteiro do teste das funções e procedures alteradas

### Prazo de entrega

**3 meses após a entrega do Pacote 1.**

---

## CUSTO EM HORAS

**Atividades consideradas:**
- Desenvolvimento
- Teste de fornecedor
- Apoio à homologação do Itaú
- Apoio à validação pós-implantação

**Entregáveis:** conforme apresentado nas páginas de Escopo.

**Rollback / disaster recovery:** conforme apresentado nas páginas de Escopo.

**Esforço:**

| Adaptação do K1 ao CNPJ alfanumérico | custo (horas) |
|---|---|
| | **2.600** |
