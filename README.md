# cblc-custo
Coletar e utilizar os custos de aquisicao informados no arquivo de saldos da CBLC.

O arquivo de saldos da CBLC, denominado ESGX, atualmente já é lido pelo sistema H1 para obtenção dos saldos de cada um dos cotistas (também denominados pelo termo 'Investidor') custodiados naquela depositaria (CBLC, atualmente denominada B3/RV), saldos esses que os cotistas têm nos ativos que sejam custodiados pelo Itaú, e portanto estejam sendo processados pelo sistema H1.
Esses ativos na verdade são Subclasses (no H1 também denominadas pelo termo 'Tipo de Cota', ou simplesmente 'Cota') de Classes (no H1 também denominadas pelo termo 'Fundo') que estejam listadas na CBLC, ou seja, que são negociadas no mercado secundário. E especificamente de Classes que estejam sob a escrituração Itaú e portanto processadas pelo sistema H1
Todos os cotistas são apresentados no arquivo ESGX, com suas informações cadastrais, os saldos de cotas que detém, e os custos de aquisição das compras que constituíram seus saldos. Isso para cada Subclasse em que tenham saldo de cotas maior que zero.

O arquivo organiza-se através de 3 tipos de registro, além do header e trailer. São eles:
1- Registro tipo 1: dados cadastrais do cotista;
2- Saldo total de cotas que o cotista tem da Subclasse em questão;
3- Abertura do saldo de cotas nas diversas aquisições realizadas pelo cotista, que ainda não foram totalmente vendidas, cujos saldos remanescentes somados correspondem ao saldo total do cotista apresentado no registro tipo 2. E apresentando também, para cada aquisição, o custo unitário com que foi realizada.

Esses arquivos são lidos diariamente, e apresentam os saldos para o dia a que correspondem. Esse dia a que o arquivo corresponde é denominado 'Data de Movimento', e em geral é informado num campo denominado 'DTMOV'.

O H1 já lê e processa diariamente esses arquivos, mas aproveita somente os registros tipo 1 e 2, coletando os dados cadastrais dos cotistas e os saldos totais deles em cada Subclasse em que eles figurem.

A necessidade agora surgida é a coleta e o uso da informação de custo de aquisição, das aquisições que compõem o saldo total do cotista na Subclasse.

A intenção inicial é o desenvolvimento de um serviço, com abordagem arquitetônica DDD, com criacao de endpoints via OpenAPI expostos via Swagger, que colete os dados do registro tipo 3 e os armazene numa tabela no banco de dados SQL do H1. O identificador do registro dessa tabela será a composição do identificador da Subclasse (campo TBPAP_CDISIN, da tabela TBH1PAP2), com o identificador do cotista (campo TBINV_CDINV da tabela TBH1INV2), e com a Data de movimento do arquivo ESGX lido.

A vinculação do cotista vindo no ESGX com a tabela TBH1INV2 se dá pelo CPF/CNPJ do investidor informado pelo arquivo, que corresponde ao campo TBINV_CDCPFCNPJ do arquivo TBH1INV2. E a vinculação do registro do arquivo com a tabela TBH1PAP2 se dá pelo campo ISIN do arquivo com o campo TBPAP_CDISIN da tabela TBH1PAP2.
