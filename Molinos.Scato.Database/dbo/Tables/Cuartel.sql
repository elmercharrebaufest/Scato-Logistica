CREATE TABLE [dbo].[Cuartel]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Codigo] NVARCHAR(10) NOT NULL, 
    [VinedoPropio_Id] INT NOT NULL,
	[Activo] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_dbo.Cuartel] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_dbo.Cuartel_dbo.Vinedo.Vinedo_Id] FOREIGN KEY ([VinedoPropio_Id]) REFERENCES [Vinedo]([Id])
)
