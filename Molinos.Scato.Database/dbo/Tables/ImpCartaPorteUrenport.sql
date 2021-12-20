CREATE TABLE [dbo].[ImpCartaPorteUrenport] (
    [Id]                    INT             NOT NULL,
	[FotoRutaDestino]				NVARCHAR (200)    NULL,
    CONSTRAINT [PK_dbo.ImpCartaPorteUrenport] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpCartaPorteUrenport_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);