CREATE TABLE [dbo].[Comercial]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Descripcion] VARCHAR(30) NOT NULL, 
    [CodigoSap] VARCHAR(20) NOT NULL Unique, 
    [Activo] BIT NOT NULL DEFAULT 1
)
