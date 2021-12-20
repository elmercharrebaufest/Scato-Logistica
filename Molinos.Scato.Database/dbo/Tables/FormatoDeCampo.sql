CREATE TABLE [dbo].[FormatoDeCampo]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Campo_Id] INT  NOT NULL,
	[FormatoDeImpresion_Id] INT NOT NULL,
    [Letra_Id] INT  NOT NULL,
	[Alineacion] INT  NOT NULL,
	[Fila] INT  NOT NULL,
	[Columna] INT  NOT NULL,
	[Tamaño] INT  NOT NULL,
	[Negrita] BIT NULL, 
	[Cursiva] BIT NULL, 
	[Subrayado] BIT NULL,
	[Texto] NVARCHAR (100)  NULL,
	[EsColumna] BIT NULL default 0,
	[TipoDeCampo]	int NOT NULL default 0,
	CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.Letra_Letra_Id] FOREIGN KEY ([Letra_Id]) REFERENCES [dbo].[Letra] ([Id]),
	CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.Campo_Campo_Id] FOREIGN KEY ([Campo_Id]) REFERENCES [dbo].[Campo] ([Id]),
	CONSTRAINT [FK_dbo.FormatoDeCampo_dbo.FormatoDeImpresion_FormatoDeImpresion_Id] FOREIGN KEY ([FormatoDeImpresion_Id]) REFERENCES [dbo].[FormatoDeImpresion] ([Id])

)
