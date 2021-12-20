CREATE TABLE [dbo].[MaterialPuerto]
(
	[Id]  INT IDENTITY (1, 1) NOT NULL, 
    [Descripcion] NVARCHAR(MAX) NULL, 
    [DescripcionCorta] NVARCHAR(50) NULL,
    [CodigoSap] NVARCHAR(50) NULL, 
    [Almacen_Id] INT NULL,
    [EsLiquido] BIT NOT NULL DEFAULT 0, 
    [Color] NVARCHAR(7) NOT NULL DEFAULT '#000000',
    CONSTRAINT [PK_MaterialPuerto] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_dbo.MaterialPuerto_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id])
)