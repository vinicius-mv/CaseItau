-- Tabela: TIPO_FUNDO
-- Tipos de fundos existentes
CREATE TABLE "TIPO_FUNDO" (
    "CODIGO"  INT          NOT NULL,
    "NOME"    VARCHAR(20)  NOT NULL,
    CONSTRAINT PK_TIPO_FUNDO PRIMARY KEY ("CODIGO")
);

-- Tabela: FUNDO
-- Registros relacionados ao cadastro de fundos
CREATE TABLE "FUNDO" (
    "CODIGO"      VARCHAR(20)     NOT NULL,
    "NOME"        VARCHAR(100)    NOT NULL,
    "CNPJ"        VARCHAR(14)     NOT NULL,
    "CODIGO_TIPO" INT             NOT NULL,
    "PATRIMONIO"  NUMERIC         NULL,
    CONSTRAINT PK_FUNDO         PRIMARY KEY ("CODIGO"),
    CONSTRAINT UQ_FUNDO_CNPJ    UNIQUE      ("CNPJ"),
    CONSTRAINT FK_FUNDO_TIPO_FUNDO
        FOREIGN KEY ("CODIGO_TIPO") REFERENCES "TIPO_FUNDO" ("CODIGO")
);