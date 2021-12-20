CREATE TABLE [dbo].[AjusteDeDiferenciasEnRedespachosTransmisionASap] (
    [Id]           INT            NOT NULL,
	[Almacen]  NVARCHAR (100) NULL,
	[Cantidad]  NVARCHAR (100) NULL,
	[Centro]  NVARCHAR (100) NULL,
	[CeCo]  NVARCHAR (100) NULL ,
	[ClaseExpedicion]  NVARCHAR (100) NULL,
	[NroDocumento]  NVARCHAR (100) NULL,
	[FechaContab]  NVARCHAR (100) NULL,
	[FechaDoc]  NVARCHAR (100) NULL,
	[Material]  NVARCHAR (100) NULL,
	[Patente]  NVARCHAR (100) NULL,
	[UniMed]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.AjusteDeDiferenciasEnRedespachosTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AjusteDeDiferenciasEnRedespachosTransmisionASap_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO