CREATE TABLE [dbo].[OrdenCargaFas] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [PatenteCamion]       NVARCHAR (10) NOT NULL,
    [PatenteAcoplado]     NVARCHAR (10) NULL,
    [Cliente_Id]          INT           NULL,
    [Material_Id]         INT           NOT NULL,
    [TipoComercial_Id]    INT           NOT NULL,
    [Transportista_Id]    INT           NULL,
    [Chofer_Id]           INT           NULL,
    [ValidaCompliance]    BIT           NOT NULL,
    [NumeroOrden]         NVARCHAR (10) NOT NULL,
    [Recorrido_Id]        INT           NOT NULL,
    [KmRecorrer]          NVARCHAR (10) NULL,
    [LocalidadDestino_Id] INT           NULL,
    [EsExtranjero]        BIT           NULL,
    CONSTRAINT [PK_dbo.OrdenCargaFas] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Cliente_Cliente_Id] FOREIGN KEY ([Cliente_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Localidad_Localidad_Id] FOREIGN KEY ([LocalidadDestino_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaFas_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenCargaFas]([Recorrido_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

