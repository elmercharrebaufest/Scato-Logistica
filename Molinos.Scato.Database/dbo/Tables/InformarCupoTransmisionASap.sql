CREATE TABLE [dbo].[InformarCupoTransmisionASap] (
    [Id]                         INT            NOT NULL,
    [NumeroCartaPorte]           NVARCHAR (100) NULL,
    [CodigoCupo]                 NVARCHAR (100) NULL,
    [FechaIngreso]               NVARCHAR (100) NULL,
    [HoraIngreso]                NVARCHAR (100) NULL,
    [TitularCpCodigoSap]         NVARCHAR (100) NULL,
    [TitularCpDescripcion]       NVARCHAR (100) NULL,
    [RtteComercialCodigoSap]     NVARCHAR (100) NULL,
    [RtteComercialDescripcion]   NVARCHAR (100) NULL,
    [CorredorCodigoSap]          NVARCHAR (100) NULL,
    [CorredorDescripcion]        NVARCHAR (100) NULL,
    [AgenteDeComprasCodigoSap]   NVARCHAR (100) NULL,
    [AgenteDeComprasDescripcion] NVARCHAR (100) NULL,
    [DestinatarioCodigoSap]      NVARCHAR (100) NULL,
    [DestinatarioDescripcion]    NVARCHAR (100) NULL,
    [EstablecimientoCodigo]      NVARCHAR (100) NULL,
    [CentroId]                   NVARCHAR (100) NULL,
    [CentroDescripcion]          NVARCHAR (100) NULL,
    [MaterialCodigoSap]          NVARCHAR (100) NULL,
    [MaterialDescripcion]        NVARCHAR (100) NULL,
    [FechaTara]                  NVARCHAR (100) NULL,
    [HoraTara]                   NVARCHAR (100) NULL,
    [FechaEgreso]                NVARCHAR (100) NULL,
    [HoraEgreso]                 NVARCHAR (100) NULL,
    [Rechazado]                  NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.InformarCupoTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.InformarCupoTransmisionASap_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);


GO