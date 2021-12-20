CREATE TABLE [dbo].[ImpIdentificacionMicromuestra] (
    [Id]                     INT             NOT NULL,
	[Centro]				NVARCHAR (50)    NULL,
	[Material]				NVARCHAR (50)    NULL,
	[NroMuestra]			NVARCHAR (50)    NULL,
	[NumeroDeOrden]			NVARCHAR (50)    NULL,
	[Proveedor]				NVARCHAR (50)    NULL,
	[Humedad]				NVARCHAR (50)    NULL,
    [NroCasillero]			NVARCHAR (11)	 NULL, 
    [TipoMicromuestra] INT NULL, 
    CONSTRAINT [PK_dbo.ImpIdentificacionMicromuestra] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpIdentificacionMicromuestra_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_ImpIdentificacionMicromuestra_TipoMicromuestra] FOREIGN KEY ([TipoMicromuestra]) REFERENCES [dbo].[TipoMicroMuestra]([Id])
);