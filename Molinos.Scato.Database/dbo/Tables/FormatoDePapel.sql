CREATE TABLE [dbo].[FormatoDePapel]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Descripcion] NVARCHAR(50) NOT NULL,
	[Ancho] INT NOT NULL,
	[Alto] INT NOT NULL, 
    [CodigoTipoPapel] INT NULL,
)
