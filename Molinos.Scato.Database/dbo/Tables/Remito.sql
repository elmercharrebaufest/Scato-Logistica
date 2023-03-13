CREATE TABLE [dbo].[Remito] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [OrdenDeDescarga]    VARCHAR (15) NOT NULL,
    [FechaOD]            DATE         NOT NULL,
    [PatenteCamion]      VARCHAR (10) NOT NULL,
    [PatenteAcoplado]    VARCHAR (10) NULL,
    [TipoComercial_Id]   INT          NOT NULL,
    [CentroOrigen_Id]    INT          NULL,
    [ProveedorOrigen_Id] INT          NULL,
    [Transportista_Id]   INT          NOT NULL,
    [Chofer_Id]          INT          NOT NULL,
    [OrdenRemito]        VARCHAR (15) NOT NULL,
    [Material_Id]        INT          NOT NULL,
    [DocLegalRemito]     VARCHAR (15) NULL,
    [CodigoAnexo]        VARCHAR (10) NULL,
    [AcuerdoMarco]       VARCHAR (15) NULL,
    [PesoTaraOrigen]     INT          NULL,
    [PesoBrutoOrigen]    INT          NULL,
    [PesoNetoOrigen]     INT          NULL,
    [Recorrido_Id]       INT          NULL,
    [KmRecorrer]         INT          NULL,
    [Procedencia_Id]     INT          NULL,
    [Cosecha]            VARCHAR (5)  NULL,
    [CodEstab]           VARCHAR (6)  NULL,
    [EsExtranjero]       BIT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Remito_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroOrigen_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Procedencia_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([ProveedorOrigen_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Remito_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    CONSTRAINT [FK_dbo.Remito_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
    UNIQUE NONCLUSTERED ([OrdenDeDescarga] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[Remito]([Recorrido_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

