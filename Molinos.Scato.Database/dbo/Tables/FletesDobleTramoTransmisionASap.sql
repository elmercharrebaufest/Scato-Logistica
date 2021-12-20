CREATE TABLE [dbo].[FletesDobleTramoTransmisionASap] (
    [Id]               INT NOT NULL,
	[Secuencia] NVARCHAR (100) NULL,
	[Almacen] NVARCHAR (100) NULL,
	[Cargador] NVARCHAR (100) NULL,
	[Centro] NVARCHAR (100) NULL,
	[Chofer] NVARCHAR (100) NULL,
	[EntradaOSalida] NVARCHAR (100) NULL,
	[FechaEgreso] NVARCHAR (100) NULL,
	[FechaIngreso] NVARCHAR (100) NULL,
	[KmRecorridos] NVARCHAR (100) NULL,
	[Material] NVARCHAR (100) NULL,
	[Neto] NVARCHAR (100) NULL,
	[CartaPorte] NVARCHAR (100) NULL,
	[Patente] NVARCHAR (100) NULL,
	[Procedencia] NVARCHAR (100) NULL,
	[ProvProc] NVARCHAR (100) NULL,
	[Transportista] NVARCHAR (100) NULL,
	[HoraEgreso] NVARCHAR (100) NULL,
	[HoraIngreso] NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.FletesDobleTramoTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.FletesDobleTramoTransmisionASap_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);
GO
