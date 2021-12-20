CREATE TABLE [dbo].[EgresoSinFleteFasonesTransmisionASap] (
    [Id]				INT       NOT NULL,
	[Almacen]			NVARCHAR (20)  NULL,
	[CuitTransportista] NVARCHAR (20) NULL,
	[CuitClienteDestinatario] NVARCHAR (20) NULL,
	[Cantidad]			NVARCHAR (20)  NULL,
	[Centro]			NVARCHAR (20)  NULL,
	[DocLegal]			NVARCHAR (40) NULL,
	[FechaCon]			NVARCHAR (50) NULL,
	[FechaDoc]			NVARCHAR (50) NULL,
	[CodigoMaterial]	NVARCHAR (10) NULL,
	[NombreChofer]		NVARCHAR (50) NULL,
	[NombreTransportista]	NVARCHAR (100) NULL,
	[NroDocumentoChofer]	NVARCHAR (50) NULL,
	[PatenteCamion]			NVARCHAR (10) NULL,        
	[PatenteAcoplado]		NVARCHAR (10) NULL,
	[TipoDocumentoChofer]	NVARCHAR (10) NULL,
    CONSTRAINT [PK_dbo.EgresoSinFleteFazonesTransmisionASapTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.EgresoSinFleteFazonesTransmisionASapTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO