CREATE TABLE [dbo].[ImpControlDeCarga] (
    [Id]                INT              NOT NULL,
	[Centro]			NVARCHAR (50)    NOT NULL,
	[NumeroControl]	    NVARCHAR (20)    NOT NULL,
	[FechaDocumentoDeIngreso]			DATETIME    NOT NULL,
	[PesoBruto]			DECIMAL(18, 2) NULL,
	[PesoTara]			DECIMAL(18, 2) NULL,
	[PesoNeto]			DECIMAL(18, 2) NULL,
	[TotalDescargado]	DECIMAL(18, 2) NULL,
	[Diferencia]		DECIMAL(18, 2) NULL,
    CONSTRAINT [PK_dbo.ImpControlDeCarga] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ImpControlDeCarga_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);