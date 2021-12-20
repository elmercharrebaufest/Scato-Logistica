CREATE TABLE [dbo].[DatosDeWorkflow] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
	[Patente]				NVARCHAR (50)    NULL,
	[PatenteAcoplado]		NVARCHAR (50)    NULL,
	[FechaCalado]			DATETIME NULL, 
	[MaterialDescripcion] NVARCHAR(50) NULL,
	[NumeroDocumentoIngreso]		NVARCHAR (50)    NULL,
	[Humedad]				NVARCHAR (50)    NULL,
	[Calidad]				NVARCHAR (50)    NULL,
	[ImpResumenHojaDeRuta_Id] INT  NOT NULL,
    CONSTRAINT [PK_dbo.DatosDeWorkflow] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.DatosDeWorkflow_dbo.ImpResumenHojaDeRuta_ImpResumenHojaDeRuta_Id] FOREIGN KEY ([ImpResumenHojaDeRuta_Id]) REFERENCES [dbo].[ImpResumenHojaDeRuta] ([Id])
);