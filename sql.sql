select * from usuario
select * from solicitacao

-- Script Date: 11/04/2025 11:10  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Item] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Descricao] nvarchar(2147483647) NULL COLLATE NOCASE
, [ValorUnitario] float NULL
, [Imagem] image NULL
);
CREATE UNIQUE INDEX [Item_Item_Descricao] ON [Item] ([Descricao] ASC);

CREATE TABLE [ItemSolicitacao] (
  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [idSolicitacao] bigint NULL
, [idItem] bigint NULL
, [idUnidadeMedida] bigint NULL
, [Quantidade] float NULL
, [ValorUnitario] float NULL
, [ValorTotal] float NULL
);
CREATE UNIQUE INDEX [ItemSolicitacao_Item_Solicitacao] ON [Solicitacao] ([idSolicitacao]);

