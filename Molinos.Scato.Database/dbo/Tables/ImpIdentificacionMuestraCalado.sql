CREATE TABLE [dbo].[ImpIdentificacionMuestraCalado] (
    [Id]               INT           NOT NULL,
    [Centro]           NVARCHAR (50) NULL,
    [NumeroCartaPorte] NVARCHAR (50) NULL,
    [PesoNeto]         NVARCHAR (50) NULL,
    [Humedad]          NVARCHAR (50) NULL,
    [Procedencia]      NVARCHAR (50) NULL,
    [FechaCalado]      DATETIME      NOT NULL,
    [NombreUsuario]    NVARCHAR (50) NULL,
    [NumeroDeOrden]    NVARCHAR (50) NULL,
    CONSTRAINT [PK_dbo.ImpIdentificacionMuestraCalado] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ImpIdentificacionMuestraCalado_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);

