CREATE TABLE [dbo].[DocumentoDeImpresion]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Codigo]                  NVARCHAR (40) NOT NULL,
	[Descripcion]             NVARCHAR (40) NOT NULL,
	[DescripcionCorta]        NVARCHAR (40) NOT NULL,
)
