CREATE TABLE [dbo].[ImpDocumentoDeEntrada] (
    [Id]                INT              NOT NULL,
	[Centro]			NVARCHAR (50)    NULL,
	[Fecha]				DATETIME NOT NULL, 
	[NumeroDeIngreso]	NVARCHAR (50)    NULL,
	[FechaDocumentoDeIngreso]		DATETIME NOT NULL, 
    CONSTRAINT [PK_dbo.ImpDocumentoDeEntrada] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpDocumentoDeEntrada_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);