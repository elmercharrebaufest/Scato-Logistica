CREATE TABLE [dbo].[ImpIdentificacionEnvioLoteACamara] (
    [Id]                     INT              NOT NULL,
	[Centro]					NVARCHAR (50)    NULL,
	[NumeroDeMuestra]			NVARCHAR (50)    NULL,
	[FechaCalado]				DATETIME NOT NULL, 
	[NumeroDeOrden]				NVARCHAR (50)    NULL,
	[Precinto]					NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.ImpIdentificacionEnvioLoteACamara] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpIdentificacionEnvioLoteACamara_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);