CREATE TABLE [dbo].[ImpGaritaSalida] (
    [Id]                      INT             NOT NULL,
    [Centro]                  NVARCHAR (50)   NULL,
    [NumeroDocumento]         NVARCHAR (50)   NULL,
    [PatenteAcoplado]         NVARCHAR (50)   NULL,
    [NumeroDeTarjetaAsignada] NVARCHAR (50)   NULL,
    [MaterialDesc]            NVARCHAR (50)   NULL,
    [EsSustentable]           BIT             NULL,
    [Patente]                 NVARCHAR (50)   NULL,
    [Calle]                   NVARCHAR (50)   NULL,
    [Hidraulicas]             NVARCHAR (1000) NULL,
    [Almacen]                 NVARCHAR (50)   NULL,
    [BalanzaTara]             NVARCHAR (50)   NULL,
    [Humedad]                 NVARCHAR (50)   NULL,
    [Calidad]                 NVARCHAR (50)   NULL,
    [FechaCalado]             NVARCHAR (50)   NULL,
    [ProteinaAlta]            NVARCHAR (50)   NULL,
    [ProteinaBaja]            NVARCHAR (50)   NULL,
    CONSTRAINT [PK_dbo.ImpGaritaSalida] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ImpGaritaSalida.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);

