CREATE TABLE [dbo].[Balanza] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (30) NOT NULL,
    [Color]  NVARCHAR(MAX) NOT NULL,
    [TipoBalanza]   INT            NOT NULL,
    [CentroEmisor] INT NOT NULL, 
    [ToleranciaOrigen] INT NULL, 
    [ToleranciaRechazo] INT NULL, 
    [ToleranciaxMil] DECIMAL NULL, 
    [ToleranciaIndianapolis] INT NOT NULL,
    [CodigoCabezal] NVARCHAR(MAX) NOT NULL, 
    [TipoAcceso] INT NULL, 
    [MaximoValorCereo] INT NOT NULL, 
    [Modalidad] INT NOT NULL,  
    [EstaEnCero] BIT NOT NULL DEFAULT 0, 
	[Centro_Id] INT  NOT NULL,
    [NombreCorto] NVARCHAR(8) NULL, 
    [PuestoDeTrabajo] NVARCHAR(50) NULL, 
    [Modelo] NVARCHAR(30) NULL, 
    [NroSerie] NVARCHAR(30) NULL, 
	[CodigoLongitud] NVARCHAR(30) NULL, 
	[CodigoLatitud] NVARCHAR(30) NULL, 
	[CertificadoDeHabilitacion] NVARCHAR(30) NULL, 
	[VencimientoDeCertificado] DATETIME NULL,
	[CodigoLot] NVARCHAR(30) NULL, 
	[TipoVehiculo] INT NOT NULL default 0,
    [EsExportacion] BIT NOT NULL DEFAULT 0,
    [Desactivado] BIT NOT NULL DEFAULT 0, 
    [ContingenciaExcedentesHabilitada] BIT NOT NULL DEFAULT 0,
    [ToleranciaExcedida] INT NULL, 
    CONSTRAINT [PK_dbo.Balanza] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Balanza_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [FK_dbo.Balanza_dbo.TipoPesada_TipoPesada_Id] FOREIGN KEY ([Modalidad]) REFERENCES [dbo].[TipoPesada] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Balanza]([Centro_Id] ASC);
GO