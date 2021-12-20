CREATE TABLE [dbo].[ImpSolicitudDeAnalisis] (
    [Id]						INT              NOT NULL,
	[Centro]					NVARCHAR (50)    NULL,
	[NumeroAnalisis]			NVARCHAR (50)    NULL,
	[Material]					NVARCHAR (50)    NULL,
	[CaracteristicaDeCalidad]	NVARCHAR (1000)    NULL,
	[FechaCalado]				DATETIME NOT NULL,
	[NumeroDeOrden]				NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.ImpSolicitudDeAnalisis] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpSolicitudDeAnalisis_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);