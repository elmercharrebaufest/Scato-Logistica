CREATE TABLE [dbo].[ImpReciboMunicipalImportacion] (
    [Id]                    INT             NOT NULL,
	[Ordenanza]				NVARCHAR (50)    NULL,
	[Valor]				NVARCHAR (50)    NULL,
	[NroDocumentoLegal]			NVARCHAR (50)    NULL,
	[TicketNro]			NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.ImpReciboMunicipalImportacion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpReciboMunicipalImportacion_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);