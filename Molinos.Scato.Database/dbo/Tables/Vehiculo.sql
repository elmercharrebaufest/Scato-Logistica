CREATE TABLE [dbo].[Vehiculo] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [Patente]              VARCHAR (10)  NOT NULL,
    [PatenteAcoplado]      VARCHAR (10)  NULL,
    [PesoTaraOrigen]       INT           NULL,
    [PesoBrutoOrigen]      INT           NULL,
    [PesoNetoOrigen]       INT           NULL,
    [NumeroVehiculo]       INT           NOT NULL,
    [CartaPorte_Id]        INT           NOT NULL,
    [TipoVehiculo]         INT           NOT NULL,
    [DocumentoInternoSap]  NVARCHAR (40) NULL,
    [NumeroDeDocumentoSap] NVARCHAR (40) NULL,
    [PatenteAcoplado2]     VARCHAR (10)  NULL,
    CONSTRAINT [PK_dbo.Vehiculo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Vehiculo_dbo.CartaPorte_CartaPorte_Id] FOREIGN KEY ([CartaPorte_Id]) REFERENCES [dbo].[CartaPorte] ([Id])
);

