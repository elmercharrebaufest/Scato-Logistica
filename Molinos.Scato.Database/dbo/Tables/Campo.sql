CREATE TABLE [dbo].[Campo]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Descripcion]             NVARCHAR (80) NOT NULL,
	[Direccion]             NVARCHAR (80) NOT NULL,
	[TituloOncca] NVARCHAR (80),
)
