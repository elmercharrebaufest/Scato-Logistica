CREATE TABLE [dbo].[Sugerencia]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [TextoSugerencia] VARCHAR(4000) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [CentroId] INT NOT NULL,
    [NombreUsuario] VARCHAR(40) NOT NULL, 
	[Url] VARCHAR(50) NOT NULL,

	CONSTRAINT [FK_dbo.Sugerencia_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id])
);