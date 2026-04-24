# Sistema H1 – Informes de cotistas Cetipados
**Escopo** | V1 12.02.2026

---

## INTRODUÇÃO

O processo H1 de envio ao DF das informações relativas aos Informes de Rendimentos atualmente aplica-se somente aos cotistas da custódia B3/RV (CBLC) e aos cotistas custodiados no ambiente escritural. Os cotistas do ambiente CETIP não participam desse processo, dada a pouca relevância da presença de fundos estruturados/fechados no ambiente Cetip.

Ressalte-se que o H1 já acolhe os cotistas dos fundos fechados da Cetip, recebendo seus saldos diários através dos arquivos de 'saldos totais' e 'saldos modificados', e utiliza esses saldos para o processo de 'conciliação do lastro Cetip' e para apresentação nos relatórios de saldos do H1 e do site YU. Mas o H1 não conhece os pagamentos individuais que esses cotistas recebem, e não contempla esses cotistas no processo de Informes de Rendimentos.

Por conta de recente perspectiva de adoção intensa do ambiente Cetip para novos fundos estruturados/fechados, e mesmo para fundos atualmente na CBLC, priorizou-se a capacitação do H1 para incluir os cotistas Cetipados no serviço de Informes de Rendimentos.

Essa capacitação do H1 exige, essencialmente, as seguintes providências:

- Regularização de alguns dados cadastrais dos cotistas Cetip (cadastros vindos nos arquivos de saldos da Cetip), necessários para criação da Conta Virtual de cada cotista e para determinar o Código de Retenção dos pagamentos;
- Criação das Contas Virtuais (também denominadas 'Contas RUC') para cotistas Cetip;
- Recebimento dos valores de cada pagamento recebido pelos cotistas Cetip;
- Liberação do envio de cotistas Cetip no processo de integração com o DF, operado pelo sistema EU8.

Nesse documento a Wise House propõe uma especificação para essa capacitação do H1, e apresenta a estimativa de esforço para esse desenvolvimento.

---

## VERSÕES DO DOCUMENTO

- **v1** 12.02.2026 – Wise House – Elaboração original.

---

## EVENTOS DE LEVANTAMENTO

- Sucessivas reuniões realizadas entre 30/1/26 e 10/2/26, com integrantes das áreas de tecnologia, de produto e de produção operacional do Itaú, e equipe da Wise House. Nessas calls o Itaú apresentou a demanda enfatizando a urgência e propondo pragmatismo na busca da solução. A Wise House inicialmente esclareceu sobre a complexidade de uma implementação plena, como a implementação já presente no processo para os fundos da CBLC. E posteriormente, buscando atender a urgência e pragmatismo solicitados, apresentou solução simplificada que foi reconhecida pelo Itaú como suficiente e foi adotada.

- Email *'H1 - Fundos Cetipados - Envio de Informes'* enviado pela Wise House em 4/2/26 19:12, apresentando, em análise preliminar, o conjunto de alterações e dependências para implementação de solução plena, similar à atual implementação desse processo para a CBLC. Essa implementação plena, além de proporcionar os Informes, ela também oferece o cálculo preciso do pagamento via LK para a depositária, baseando esse valor na soma dos pagamentos na depositária, em lugar de basear esse pagamento em cálculos aproximados do pagamento total no Lastro Cetip.

- Email *'Estimativa de horas - Projeto H1 - Envio de pagamento Cetip para DF'* enviado pela Wise House em 9/2/26 18:33 apresentando a solução 'pragmática', considerada satisfatória e adotada pelo Itaú, e viável no prazo necessário.

---

## ESCOPO

A solução é composta por quatro conjuntos coesos de implementação, apresentados a seguir.

---

### 1 – APERFEIÇOAMENTO DO CADASTRO DO COTISTA CETIP

- Na versão atualmente lida pelo H1, os arquivos de saldos Cetip (DSTOTAISxxxx e DSMODIFxxxx) não informam se o identificador fiscal (CPF/CNPJ) é um CPF ou CNPJ. Dado isso, essa implementação vai adotar a regra de reconhecimento desse tipo através do número de caracteres. Se menor ou igual a 11, o identificador é assumido como CPF, caso contrário é assumido como CNPJ. Sem nenhuma dinâmica de validação.

- Dado que os arquivos também não indicam se o cotista é PF ou PJ, esse atributo será decorrente da mesma regra acima, reconhecendo como PF os que vieram com CPF, e reconhecendo os outros como PJ.

- Para saber se o cotista é Residente ou Não residente, serão utilizados os valores dos campos *'Data início do Período da Situação de Residente no Exterior'* e *'Data final do Período da Situação de Residente no Exterior'* presentes no layout dos arquivos de saldos.
  **Premissa:** considera-se que esses campos estejam devidamente preenchidos nos arquivos de saldos Cetip.

- O Tipo de Remessa do cotista, se retida ou se liberada para envio postal, não é informado pelos arquivos. Essa implementação assumirá o Tipo de Remessa dos investidores como 'Correio', a exemplo do que já ocorre hoje no processo CBLC.

- Atualmente os cotistas Cetip não participam do processo de solicitação de Conta Virtual, seja pelas carências cadastrais que são acima mencionadas e mitigadas, seja pela desnecessidade. Essa Conta Virtual no entanto é essencial para o envio do cotista ao processo de Informes do DF. Esse projeto estenderá aos cotistas Cetip o processo de obtenção e atualização de Contas Virtuais junto ao sistema CC, da mesma forma que hoje é feito para cotistas do escritural e cotistas da CBLC.

---

### 2 – TELA DE REVISÃO DE DADOS CADASTRAIS DE COTISTAS CETIP

> **Nota:** Os cadastros de cotistas CETIP hoje já estão presentes no sistema, embora sem os aperfeiçoamentos descritos acima. São criados e alterados apenas pela leitura dos arquivos Cetip de saldos, são consultáveis pelo usuário mas não são passíveis de edição (assim como não se permite edição também para os cotistas da CBLC). Aqui se propõe a criação de um recurso de edição desses cotistas Cetip, mas numa versão simplificada, e permitindo alteração apenas de alguns atributos, conforme abaixo.

**Definição:**

- Tela para edição do cadastro de cotistas Cetip já presentes no sistema, exclusivamente para alteração dos campos **Residente/Não residente**, **País de Residência**, **Tipo de Remessa** e **Tipo de Tributação**.
- Tela implementada no `Ferramentas.exe`
- Sem mecanismo de feito/conferido
- Sem acionamento do processo de solicitação/alteração de Conta Virtual no encerramento da alteração de cada cotista.
- **Nota:** a alteração do Tipo de Remessa do cotista, se ocorrida, deve ser comunicada ao sistema CC como atualização da Conta Virtual já existente para o cotista. Esse processo de atualização da Conta Virtual não será aqui provocada, e dependerá de processo hoje já existente no Ferramentas, executado pela área usuária no fechamento do mês.
- Permitirá apenas a alteração de cotistas Cetip já existentes, sem permitir criação ou exclusão de cadastro.

---

### 3 – LIBERAÇÃO DE COTISTAS CETIP NA INTERFACE COM O DF

Trata-se de alteração nas procedures **EANL** (pagamentos) e **ESMA** (saldos), presentes no banco do H1, feitas a quatro mãos entre a Wise House e o sistema EU8.

É o EU8 que aciona essas procedures e controla o processo de entrega dos pagamentos (EANL) e dos saldos (ESMA) do H1 para o DF.

Basicamente, essa alteração resume-se à liberação de filtros, que hoje restringem o envio ao DF somente dos cotistas CBLC e dos cotistas custodiados no ambiente escritural.

**Premissa:** concordância da equipe do EU8 quanto às alterações aqui consideradas.

---

### 4 – ENTRADA EM LOTE DOS PAGAMENTOS CETIP COM RECURSO DE APOIO

> **Notas:**
> - Os processos tratados nesse módulo 4 se aplicarão somente a eventos já efetivados.
> - A saída da planilha entregue pelo H1, e a entrada da planilha após o preenchimento pela área usuária, esses dois processos serão executados via download e upload por tela do sistema.

**Descrição do processo:**

- Após cadastrado o evento (como atualmente já é feito), um botão gera uma planilha de apoio, com todos os cotistas Cetip que tenham posição no ativo e na data-base do evento, apresentando dados do evento e do ativo, dados cadastrais do cotista, e o estoque do cotista na data-base.

- Esses dados serão apenas informativos, de suporte, não produzirão nenhum cálculo.

- A planilha terá campos em aberto para a área operacional preencher, quais sejam:
  - Valor financeiro bruto do pagamento para o cotista
  - Valor do IR do pagamento
  - Código de Retenção do pagamento *(nota: esse código já virá preenchido pelo sistema que o terá calculado a partir de dados do pagamento e do investidor, mas o usuário poderá alterá-lo)*

- O usuário deverá zelar pela preservação das linhas da planilha, das colunas e seus valores, restringindo-se a alimentar os 3 campos destinados à digitação.

- Poderá eliminar registros, se desejar remover cotistas do pagamento.

- Poderá incluir registros, se desejar incluir cotista não trazido pela planilha fornecida pelo H1, mas esse cotista deverá ser do tipo Cetip.

- Essa planilha, após complementada pelo usuário, será carregada como sendo o arquivo de pagamentos CETIP para o Evento em questão.

**O arquivo a ser carregado será aceito somente se:**

- For uma planilha bem formada, correspondente à planilha que o H1 espera quanto às colunas;
- A coluna com o Código do Evento estiver inteiramente preenchida com o mesmo código.

**Crítica — cada linha da planilha sofrerá as seguintes verificações:**

- Se o cotista da linha é um cotista Cetip
- Se o cotista tem posição na data-base do evento
- Se o Código de Retenção tem valor
- Se a Conta Virtual tem valor
- Se o IR é maior que o Valor bruto

- A primeira irregularidade (Cotista não Cetip) é a mais severa, e é a única que impedirá que o registro seja processado, embora sem impedir que os outros registros sejam processados.
- As irregularidades serão apontadas pelo sistema em cada registro na tela de carga da planilha, podendo ser filtradas.
- Exceto quanto à primeira irregularidade, todas as outras não impedirão que o registro seja processado e associado ao Evento.

> **IMPORTANTE:** Cada releitura para o mesmo Evento será sempre integral, ou seja, eliminam-se todos os pagamentos carregados pela leitura anterior e o novo conteúdo é assumido integralmente.

- Após o upload e visualização da crítica, a planilha poderá ser processada, ou seja, seus registros serão incorporados ao Evento em questão, passando a figurar nos relatórios de pagamentos do H1 e do YU, e a participar do processo de envio ao DF.

- Os pagamentos Cetip assim processados também serão apresentados no **Relatório > Conciliação DIRF** usado pela área operacional para envio de informações ao processo de retenção de IR na fonte.

- **FEITO/CONFERIDO:** A solicitação desse processamento poderá ser feita somente por usuário diferente do usuário que fez o upload da planilha.

- Após ter sofrido o upload, a planilha poderá ser sempre consultada, esteja já processada ou ainda pendente de processamento. O acesso a esse conteúdo será sempre pelo Código do Evento.

---

## CUSTO EM HORAS

**Atividades consideradas:**

- Levantamento e proposta de solução
- Desenvolvimento
- Teste de fornecedor
- Apoio à homologação do Itaú

**Entregáveis:**

- Scripts SQL
- Fontes alterados da aplicação Web H1
- Fontes alterados do `Ferramentas.exe`

**Esforço Wise House:**

| **296 horas** |
|---|

---

*Wise House Engenharia de Sistemas*
