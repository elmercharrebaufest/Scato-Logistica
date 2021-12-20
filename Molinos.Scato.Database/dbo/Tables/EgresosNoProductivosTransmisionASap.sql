CREATE TABLE [dbo].[EgresosNoProductivosTransmisionASap] (
    [Id] INT NOT NULL,
	[Cliente] NVARCHAR (100) NULL,
	[FechaOrden]  NVARCHAR (50) NULL,
	[PuestoExp]  NVARCHAR (100) NULL,
	[PesoTotal]  DECIMAL(18,2) NULL,
	[Transportista]  NVARCHAR (100) NULL,
	[UnidadPeso]  NVARCHAR(20) NULL,
	[DocChofer] NVARCHAR(50) NULL,
	[TipoDocChofer] NVARCHAR(10) NULL,
	[NomChofer] NVARCHAR(100) NULL,
	[PatCamion] NVARCHAR(10) NULL,
	[PatRemolque] NVARCHAR(10) NULL,

	[Descripcion]  NVARCHAR (100) NULL,
	[Centro]  NVARCHAR (20) NULL,
	[Almacen]  NVARCHAR (20) NULL,
	[Cantidad]  DECIMAL(18,2) NULL,
	[Unidad]  NVARCHAR (20) NULL,
	[UnidadPesoItem]  NVARCHAR (20) NULL,
	[Peso] DECIMAL(18,2) NULL,
	
    CONSTRAINT [PK_dbo.EgresosNoProductivosTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.EgresosNoProductivosTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO