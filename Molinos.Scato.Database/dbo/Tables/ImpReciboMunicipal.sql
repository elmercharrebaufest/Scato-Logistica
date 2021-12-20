CREATE TABLE [dbo].[ImpReciboMunicipal] (
    [Id]                    INT             NOT NULL,
	[Ordenanza]				NVARCHAR (50)    NULL,
	[Valor]				NVARCHAR (50)    NULL,
	[NroDocumentoLegal]			NVARCHAR (50)    NULL,
	[TicketNro]			NVARCHAR (50)    NULL,
    [TipoVehiculo] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.ImpReciboMunicipal] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpReciboMunicipal_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);