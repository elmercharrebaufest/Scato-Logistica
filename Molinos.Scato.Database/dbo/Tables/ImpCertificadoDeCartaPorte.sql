CREATE TABLE [dbo].[ImpCertificadoDeCartaPorte] (
    [Id]                     INT             NOT NULL,
	[Centro]				NVARCHAR (50)    NULL,
	[Material]				NVARCHAR (50)    NULL,
	[Balanza]				NVARCHAR (50)    NULL,
	[NumeroIngreso]			NVARCHAR (50)    NULL,
	[FechaEntrada]			DATETIME NOT NULL,
	[FechaSalida]			DATETIME NOT NULL, 
	[NumeroCertificacion]	NVARCHAR (50)    NULL,
	[TipoDocumento]			NVARCHAR (50)    NULL,
	[NumeroDocumento]		NVARCHAR (50)    NULL,
	[PesoBruto]				NVARCHAR (50)    NULL,
	[PesoTara]				NVARCHAR (50)    NULL,
	[PesoNeto]				NVARCHAR (50)    NULL,
	[PesoNetoOrigen]		NVARCHAR (50)    NULL,
	[Diferencia]			NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.ImpCertificadoDeCartaPorte] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpCertificadoDeCartaPorte_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);