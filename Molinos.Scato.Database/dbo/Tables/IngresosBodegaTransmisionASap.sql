CREATE TABLE [dbo].[IngresosBodegaTransmisionASap] (
    [Id] INT NOT NULL,
	[Bins] NVARCHAR (100) NULL,
	[Calidad] NVARCHAR (100) NULL,
	[Cantidad]  DECIMAL(18, 2) NULL,
	[Centro]  NVARCHAR (100) NULL,
	[Ciu]  NVARCHAR (100) NULL,
	[Cosecha]  NVARCHAR(100) NULL,
	[Cuartel] NVARCHAR(100) NULL,
	[FechaContabilizacion] NVARCHAR(100) NULL,
	[FechaDocumento] NVARCHAR(100) NULL,
	[Finca] NVARCHAR(100) NULL,
	[Inv] NVARCHAR(100) NULL,
	[Material] NVARCHAR(100) NULL,
	[NumeroDocumento]  NVARCHAR (100) NULL,
	[NumeroNota]  NVARCHAR (100) NULL,
	[PosDocumento]  NVARCHAR (100) NULL,
	[Propio]  NVARCHAR (100) NULL,
	[SubZona]  NVARCHAR (100) NULL,
	[Tanque]  NVARCHAR (100) NULL,
	[Tenor]  NVARCHAR (100) NULL,
	[Varietal]  NVARCHAR (100) NULL,
	[Zona]  NVARCHAR (100) NULL,
	
    [TipoBinId] INT NULL, 
    CONSTRAINT [PK_dbo.IngresosBodegaTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.IngresosBodegaTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO