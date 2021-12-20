CREATE TABLE [dbo].[SubZona]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Zona_Id] INT NOT NULL, 
    [Descripcion] NVARCHAR(30) NOT NULL
	CONSTRAINT [PK_dbo.SubZona] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_dbo.SubZona_dbo.Zona.Zona_Id] FOREIGN KEY ([Zona_Id]) REFERENCES [dbo].[Zona]([Id]),
)
