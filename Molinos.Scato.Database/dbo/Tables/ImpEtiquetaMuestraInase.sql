CREATE TABLE [dbo].[ImpEtiquetaMuestraInase] (
    [Id]                INT              NOT NULL,
	[Centro]			NVARCHAR (50)    NULL,
	[ProductorCuit]		NVARCHAR (50)    NULL,
	[Cpe]				NVARCHAR (50)    NULL,
	[Material]			NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.ImpEtiquetaMuestraInase] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpEtiquetaMuestraInase_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);