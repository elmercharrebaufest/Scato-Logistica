CREATE TABLE [dbo].[MaterialPuertoCantidad]
(
	[Id]  INT IDENTITY (1, 1) NOT NULL, 
    [Cantidad] INT NULL, 
    [Embarque_Id] INT NOT NULL, 
    [MaterialPuerto_Id] INT NOT NULL, 
    [Color] NVARCHAR(7) NOT NULL DEFAULT '#000000',
    CONSTRAINT [PK_MaterialPuertoCantidad] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_dbo.MaterialPuertoCantidad_dbo.Embarque_Embarque_Id] FOREIGN KEY ([Embarque_Id]) REFERENCES [dbo].[Embarque] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MaterialPuertoCantidad_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id])
)