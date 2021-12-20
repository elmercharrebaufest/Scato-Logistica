CREATE TABLE [dbo].[ImpEtiquetaIntacta] (
    [Id]                     INT             NOT NULL,
	[Centro]				NVARCHAR (50)    NULL,
	[Material]				NVARCHAR (150)    NULL,
	[NumeroCartaPorte]			NVARCHAR (50)    NULL,
	[TipoDeAnalisis]				NVARCHAR (50)    NULL,
	[LaboratiorioCuit]				NVARCHAR (50)    NULL,
	[LaboratorioNombre]				NVARCHAR (50)    NULL,

    CONSTRAINT [PK_dbo.ImpEtiquetaIntacta] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpEtiquetaIntacta_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);