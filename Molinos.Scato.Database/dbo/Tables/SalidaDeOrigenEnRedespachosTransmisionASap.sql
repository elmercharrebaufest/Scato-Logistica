CREATE TABLE [dbo].[SalidaDeOrigenEnRedespachosTransmisionASap] (
    [Id]                  INT             NOT NULL,
    [AlmEmisor]           NVARCHAR (100)  NULL,
    [AlmReceptor]         NVARCHAR (100)  NULL,
    [Cantidad]            NVARCHAR (100)  NULL,
    [CentroEmisor]        NVARCHAR (100)  NULL,
    [CentroReceptor]      NVARCHAR (100)  NULL,
    [ClaseExpedicion]     NVARCHAR (100)  NULL,
    [CUITTransp]          NVARCHAR (100)  NULL,
    [NroDocumento]        NVARCHAR (100)  NULL,
    [FechaContab]         NVARCHAR (100)  NULL,
    [FechaDoc]            NVARCHAR (100)  NULL,
    [Lote]                NVARCHAR (100)  NULL,
    [Kilometros]          NUMERIC (18, 2) NULL,
    [Material]            NVARCHAR (100)  NULL,
    [NombreChofer]        NVARCHAR (100)  NULL,
    [NombreTransportista] NVARCHAR (100)  NULL,
    [DocChofer]           NVARCHAR (100)  NULL,
    [Patente1]            NVARCHAR (100)  NULL,
    [Patente2]            NVARCHAR (100)  NULL,
    [Precinto1]           NVARCHAR (100)  NULL,
    [Precinto2]           NVARCHAR (100)  NULL,
    [TipoDoc]             NVARCHAR (100)  NULL,
    [UniMed]              NVARCHAR (100)  NULL,
    CONSTRAINT [PK_dbo.SalidaDeOrigenEnRedespachosTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.SalidaDeOrigenEnRedespachosTransmisionASap_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);



GO