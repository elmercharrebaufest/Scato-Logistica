CREATE TABLE [dbo].[Vinedo]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [NumeroINV] NVARCHAR(8) NOT NULL, 
    [Descripcion] NVARCHAR(50) NOT NULL, 
	[Proveedor_Id] int NULL, 
	[IngresosBrutos] NVARCHAR(30) NOT NULL, 
    [CUIT_Titular] NVARCHAR(13) NULL, 
    [Discriminator] NVARCHAR(30) NULL, 
    [SubZona_Id] INT NULL, 
    [Calidad] NVARCHAR(40) NULL, 
	[CentroOperativo] NVARCHAR(40) NULL, 
    CONSTRAINT [PK_dbo.Vinedo] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_dbo.Vinedo_dbo.Proveedor.Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor]([Id]),
	CONSTRAINT [FK_dbo.Vinedo_dbo.SubZona.SubZona_Id] FOREIGN KEY ([SubZona_Id]) REFERENCES [dbo].[SubZona]([Id]),
);
