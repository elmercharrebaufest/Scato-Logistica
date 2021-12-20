CREATE TABLE [dbo].[FormatoDeImpresion]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Descripcion] NVARCHAR(50) NOT NULL,
	[FormatoDePapel_Id] INT  NOT NULL,
	[Posicion] INT  NOT NULL,
	[MargenIzquierdo] INT  NOT NULL,
	[MargenSuperior] INT  NOT NULL,
	[Filas] INT  NOT NULL,
	[Columnas] INT  NOT NULL,

	CONSTRAINT [FK_dbo.FormatoDeImpresion_dbo.FormatoDePapel_FormatoDePapel_Id] FOREIGN KEY ([FormatoDePapel_Id]) REFERENCES [dbo].[FormatoDePapel] ([Id]),
)
