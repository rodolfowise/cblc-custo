-- ============================================================================
-- Script de Criação de Tabelas para Processamento de Arquivos ESGX
-- Saldos Gerais da CBLC - Custos de Aquisição
-- ============================================================================
-- Descrição:
-- Tabela 1: TBH1ESGX_SALDOS_INVESTIDOR - Saldos totais por Investidor x ISIN (Registro Tipo 2)
-- Tabela 2: TBH1ESGX_AQUISICOES      - Detalhes de aquisições por investidor (Registro Tipo 3)
-- ============================================================================

-- Tabela 1: SALDOS TOTAIS DO INVESTIDOR POR ISIN
-- Armazena os saldos totais de cotas que cada investidor possui em cada Subclasse (ISIN)
-- Corresponde ao Registro Tipo 2 do arquivo ESGX
-- ============================================================================
CREATE TABLE TBH1ESGX_SALDOS_INVESTIDOR
(
    -- Chave Primária
    TBESLDI_CDISIN          VARCHAR(12)     NOT NULL,  -- Código ISIN da Subclasse (de TBPAP2)
    TBESLDI_CDCPFCNPJ       VARCHAR(15)     NOT NULL,  -- CPF/CNPJ do Investidor (de TBINV2)
    TBESLDI_DTMOV           DATE            NOT NULL,  -- Data de Movimento do arquivo ESGX
    
    -- Dados do Investidor (do Registro Tipo 1)
    TBESLDI_CDINVESTIDOR    NUMERIC(12)     NOT NULL,  -- ID do Investidor (TBINV_CDINV de TBINV2)
    TBESLDI_NMINVESTIDOR    VARCHAR(60)     NULL,      -- Nome do Investidor
    TBESLDI_TPPESSOA        CHAR(1)         NULL,      -- F (Física) ou J (Jurídica)
    
    -- Dados da Subclasse (do Registro Tipo 2)
    TBESLDI_NMEMISSORA      VARCHAR(12)     NULL,      -- Nome da Sociedade Emissora
    TBESLDI_VLESPEC         VARCHAR(10)     NULL,      -- Especificação
    TBESLDI_DTREF           DATE            NULL,      -- Data de Referência
    
    -- Saldo Total (do Registro Tipo 2)
    TBESLDI_QTDSALDO        NUMERIC(17,3)   NOT NULL,  -- Quantidade Total de Cotas/Ativos
    TBESLDI_VLCUSTOUNITARIO NUMERIC(19,8)   NULL,      -- Custo Unitário Médio Ponderado (calculado)
    TBESLDI_VLCUSTOTOTAL    NUMERIC(19,2)   NULL,      -- Custo Total (calculado: Qtd x Custo Unitário)
    
    -- Informações de Controle
    TBESLDI_DHREG           DATETIME        NOT NULL DEFAULT GETDATE(),  -- Data/Hora do Registro
    TBESLDI_DTULTALT        DATETIME        NOT NULL DEFAULT GETDATE(),  -- Data/Hora da Última Alteração
    TBESLDI_IDSEQARQUIVO    NUMERIC(9)      NULL,      -- Número sequencial do arquivo ESGX processado
    
    -- Restrições
    PRIMARY KEY CLUSTERED (TBESLDI_CDISIN, TBESLDI_CDCPFCNPJ, TBESLDI_DTMOV)
);

-- Índices para otimização de buscas
CREATE INDEX IDX_ESLDI_CDINVESTIDOR  ON TBH1ESGX_SALDOS_INVESTIDOR (TBESLDI_CDINVESTIDOR);
CREATE INDEX IDX_ESLDI_CDCPFCNPJ     ON TBH1ESGX_SALDOS_INVESTIDOR (TBESLDI_CDCPFCNPJ);
CREATE INDEX IDX_ESLDI_DTMOV         ON TBH1ESGX_SALDOS_INVESTIDOR (TBESLDI_DTMOV);
CREATE INDEX IDX_ESLDI_CDISIN        ON TBH1ESGX_SALDOS_INVESTIDOR (TBESLDI_CDISIN);

-- Comentários descritivos da tabela
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Saldos totais de cotas/ativos por Investidor e Subclasse (ISIN), baseado no Registro Tipo 2 do arquivo ESGX. Chave composta: ISIN + CPF/CNPJ + Data Movimento.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'TBH1ESGX_SALDOS_INVESTIDOR';


-- Tabela 2: AQUISIÇÕES DETALHADAS DO INVESTIDOR POR ISIN
-- Armazena os detalhes de cada aquisição que compõe o saldo total do investidor
-- Corresponde ao Registro Tipo 3 do arquivo ESGX
-- Uma aquisição = uma data de compra + preço de compra = uma entrada no Registro Tipo 3
-- Podem existir múltiplos registros tipo 3 para cada Registro Tipo 2
-- ============================================================================
CREATE TABLE TBH1ESGX_AQUISICOES
(
    -- Chave Primária
    TBESLAQ_CDISIN          VARCHAR(12)     NOT NULL,  -- Código ISIN da Subclasse (de TBPAP2)
    TBESLAQ_CDCPFCNPJ       VARCHAR(15)     NOT NULL,  -- CPF/CNPJ do Investidor (de TBINV2)
    TBESLAQ_DTMOV           DATE            NOT NULL,  -- Data de Movimento do arquivo ESGX
    TBESLAQ_DTAQUISICAO     DATE            NOT NULL,  -- Data de Aquisição (data da compra)
    TBESLAQ_VLPRECOACQ      NUMERIC(19,8)   NOT NULL,  -- Preço Unitário de Aquisição
    
    -- Dados do Investidor (do Registro Tipo 1)
    TBESLAQ_CDINVESTIDOR    NUMERIC(12)     NOT NULL,  -- ID do Investidor (TBINV_CDINV de TBINV2)
    TBESLAQ_NMINVESTIDOR    VARCHAR(60)     NULL,      -- Nome do Investidor
    TBESLAQ_TPPESSOA        CHAR(1)         NULL,      -- F (Física) ou J (Jurídica)
    
    -- Dados da Subclasse (do Registro Tipo 3)
    TBESLAQ_NMEMISSORA      VARCHAR(12)     NULL,      -- Nome da Sociedade Emissora
    TBESLAQ_VLESPEC         VARCHAR(10)     NULL,      -- Especificação
    TBESLAQ_DTREF           DATE            NULL,      -- Data de Referência do arquivo
    
    -- Quantidade e Custo da Aquisição (do Registro Tipo 3)
    TBESLAQ_QTDAQUISICAO    NUMERIC(17,3)   NOT NULL,  -- Quantidade de Cotas/Ativos adquiridos
    TBESLAQ_VLCUSTOTOTAL    NUMERIC(19,2)   NULL,      -- Custo Total da Aquisição (Qtd x Preço)
    
    -- Informações de Controle
    TBESLAQ_DHREG           DATETIME        NOT NULL DEFAULT GETDATE(),  -- Data/Hora do Registro
    TBESLAQ_DTULTALT        DATETIME        NOT NULL DEFAULT GETDATE(),  -- Data/Hora da Última Alteração
    TBESLAQ_IDSEQARQUIVO    NUMERIC(9)      NULL,      -- Número sequencial do arquivo ESGX processado
    
    -- Restrições
    PRIMARY KEY CLUSTERED (TBESLAQ_CDISIN, TBESLAQ_CDCPFCNPJ, TBESLAQ_DTMOV, TBESLAQ_DTAQUISICAO, TBESLAQ_VLPRECOACQ)
);

-- Índices para otimização de buscas
CREATE INDEX IDX_ESLAQ_CDINVESTIDOR  ON TBH1ESGX_AQUISICOES (TBESLAQ_CDINVESTIDOR);
CREATE INDEX IDX_ESLAQ_CDCPFCNPJ     ON TBH1ESGX_AQUISICOES (TBESLAQ_CDCPFCNPJ);
CREATE INDEX IDX_ESLAQ_DTMOV         ON TBH1ESGX_AQUISICOES (TBESLAQ_DTMOV);
CREATE INDEX IDX_ESLAQ_CDISIN        ON TBH1ESGX_AQUISICOES (TBESLAQ_CDISIN);
CREATE INDEX IDX_ESLAQ_DTAQUISICAO   ON TBH1ESGX_AQUISICOES (TBESLAQ_DTAQUISICAO);
CREATE INDEX IDX_ESLAQ_ISIN_CPFCNPJ_DTMOV ON TBH1ESGX_AQUISICOES (TBESLAQ_CDISIN, TBESLAQ_CDCPFCNPJ, TBESLAQ_DTMOV);

-- Comentários descritivos da tabela
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Detalhes de aquisições de cotas/ativos por Investidor, baseado no Registro Tipo 3 do arquivo ESGX. Múltiplas aquisições podem compor um saldo total (Tipo 2). Chave composta: ISIN + CPF/CNPJ + Data Movimento + Data Aquisição + Preço Aquisição.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'TBH1ESGX_AQUISICOES';


-- ============================================================================
-- VIEWS ÚTEIS PARA CONSULTAS E VALIDAÇÕES
-- ============================================================================

-- View 1: Custo Unitário Médio Ponderado calculado a partir das aquisições
CREATE VIEW VW_ESGX_CUSTO_UNITARIO_MEDIO AS
SELECT 
    TBESLAQ_CDISIN,
    TBESLAQ_CDCPFCNPJ,
    TBESLAQ_DTMOV,
    TBESLAQ_CDINVESTIDOR,
    SUM(TBESLAQ_QTDAQUISICAO) AS QTDSALDO_TOTAL,
    SUM(TBESLAQ_VLCUSTOTOTAL) AS VLCUSTO_TOTAL,
    CASE 
        WHEN SUM(TBESLAQ_QTDAQUISICAO) > 0 
        THEN SUM(TBESLAQ_VLCUSTOTOTAL) / SUM(TBESLAQ_QTDAQUISICAO)
        ELSE 0
    END AS VLCUSTO_UNITARIO_MEDIO
FROM TBH1ESGX_AQUISICOES
GROUP BY TBESLAQ_CDISIN, TBESLAQ_CDCPFCNPJ, TBESLAQ_DTMOV, TBESLAQ_CDINVESTIDOR;

-- View 2: Reconciliação entre Saldos (Tipo 2) e Aquisições (Tipo 3)
CREATE VIEW VW_ESGX_RECONCILIACAO AS
SELECT 
    S.TBESLDI_CDISIN,
    S.TBESLDI_CDCPFCNPJ,
    S.TBESLDI_DTMOV,
    S.TBESLDI_NMINVESTIDOR,
    S.TBESLDI_QTDSALDO AS SALDO_TIPO2_QTD,
    C.QTDSALDO_TOTAL AS AQUISICOES_TIPO3_QTD,
    ABS(S.TBESLDI_QTDSALDO - ISNULL(C.QTDSALDO_TOTAL, 0)) AS DIFERENCA_QTD,
    S.TBESLDI_VLCUSTOTOTAL AS SALDO_TIPO2_CUSTO_TOTAL,
    C.VLCUSTO_TOTAL AS AQUISICOES_TIPO3_CUSTO_TOTAL,
    C.VLCUSTO_UNITARIO_MEDIO AS CUSTO_UNITARIO_CALCULADO,
    CASE 
        WHEN ABS(S.TBESLDI_QTDSALDO - ISNULL(C.QTDSALDO_TOTAL, 0)) < 0.001 
        THEN 'OK'
        ELSE 'DIVERGÊNCIA'
    END AS STATUS_VALIDACAO
FROM TBH1ESGX_SALDOS_INVESTIDOR S
LEFT JOIN VW_ESGX_CUSTO_UNITARIO_MEDIO C 
    ON S.TBESLDI_CDISIN = C.TBESLAQ_CDISIN 
    AND S.TBESLDI_CDCPFCNPJ = C.TBESLAQ_CDCPFCNPJ 
    AND S.TBESLDI_DTMOV = C.TBESLAQ_DTMOV;

-- ============================================================================
-- SCRIPT DE LIMPEZA (OPCIONAL - para testes)
-- ============================================================================
/*
DROP VIEW IF EXISTS VW_ESGX_RECONCILIACAO;
DROP VIEW IF EXISTS VW_ESGX_CUSTO_UNITARIO_MEDIO;
DROP TABLE IF EXISTS TBH1ESGX_AQUISICOES;
DROP TABLE IF EXISTS TBH1ESGX_SALDOS_INVESTIDOR;
*/
