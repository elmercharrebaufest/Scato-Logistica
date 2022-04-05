CREATE TABLE [dbo].[FormatoDeCampo] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [Campo_Id]              INT            NOT NULL,
    [FormatoDeImpresion_Id] INT            NOT NULL,
    [Letra_Id]              INT            NOT NULL,
    [Alineacion]            INT            NOT NULL,
    [Fila]                  INT            NOT NULL,
    [Columna]               INT            NOT NULL,
    [Tamaño]                INT            NOT NULL,
    [Negrita]               BIT            NULL,
    [Cursiva]               BIT            NULL,
    [Subrayado]             BIT            NULL,
    [Texto]                 NVARCHAR (100) NULL,
    [EsColumna]             BIT            DEFAULT ((0)) NULL,
    [TipoDeCampo]           INT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.Campo_Campo_Id] FOREIGN KEY ([Campo_Id]) REFERENCES [dbo].[Campo] ([Id]),
    CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.FormatoDeImpresion_FormatoDeImpresion_Id] FOREIGN KEY ([FormatoDeImpresion_Id]) REFERENCES [dbo].[FormatoDeImpresion] ([Id]),
    CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.Letra_Letra_Id] FOREIGN KEY ([Letra_Id]) REFERENCES [dbo].[Letra] ([Id])
);


