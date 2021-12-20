CREATE TABLE [dbo].[IngresosEgresosFazonesTransmisionASap] (
    [Id]           INT            NOT NULL,
	[Almacen]  NVARCHAR (100) NULL,
	[PesoNeto]  NVARCHAR (100) NULL,
	[NumeroDocumento]  NVARCHAR (100) NULL,
	[FechaIngreso]  NVARCHAR (100) NULL,
	[Km]  DECIMAL(18, 2) NULL,
	[Localidad]  NVARCHAR (100) NULL,
	[Centro]  NVARCHAR (100) NULL,
	[Cliente]  NVARCHAR (100) NULL,
	[Material]  NVARCHAR (100) NULL,
	[Patente]  NVARCHAR (100) NULL,
	[Provincia]  NVARCHAR (100) NULL,
	[TipoMovimiento]  NVARCHAR(100) NULL,
	[Transportista]  NVARCHAR (100) NULL,
	[UnidadMedida]  NVARCHAR(100) NULL,
    [NombreChofer] NVARCHAR(100) NULL, 
    [NumeroDocumentoChofer] NVARCHAR(100) NULL, 
    [TipoDocumentoChofer] NVARCHAR(100) NULL, 
    [PatenteAcoplado] NVARCHAR(100) NULL, 
    [RecorridoId] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_dbo.IngresosEgresosFazonesTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IngresosEgresosFazonesTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO