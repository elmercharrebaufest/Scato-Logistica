CREATE TABLE [dbo].[OrdenCargaInternaFason] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [NumeroOrden]         NVARCHAR (40) NOT NULL,
    [FechaEmision]        DATETIME      NOT NULL,
    [PatenteCamion]       NVARCHAR (10) NOT NULL,
    [PatenteAcoplado]     NVARCHAR (10) NULL,
    [Transportista_Id]    INT           NULL,
    [TipoComercial_Id]    INT           NOT NULL,
    [Material_Id]         INT           NOT NULL,
    [Chofer_Id]           INT           NOT NULL,
    [Cliente_Id]          INT           NOT NULL,
    [Recorrido_Id]        INT           NOT NULL,
    [KmRecorrer]          NVARCHAR (10) NULL,
    [LocalidadDestino_Id] INT           NULL,
    [EsExtranjero]        BIT           NULL,
    [DerivadoGranarioHabilitado] BIT DEFAULT ((0)) NOT NULL,
    [PlantaDGDestino] INT NULL, 
    [OrdenDomicilioDestino] INT NULL, 
    [PagadorFlete_Id] INT NULL,
    CONSTRAINT [PK_dbo.OrdenCargaInternaFason] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Cliente_Cliente_Id] FOREIGN KEY ([Cliente_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Localidad_Localidad_Id] FOREIGN KEY ([LocalidadDestino_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInternaFason_dbo.Cliente_PagadorFlete_Id] FOREIGN KEY ([PagadorFlete_Id]) REFERENCES [dbo].[Cliente] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Transportista_Id]
    ON [dbo].[OrdenCargaInternaFason]([Transportista_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_TipoComercial_Id]
    ON [dbo].[OrdenCargaInternaFason]([TipoComercial_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[OrdenCargaInternaFason]([Material_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Chofer_Id]
    ON [dbo].[OrdenCargaInternaFason]([Chofer_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenCargaInternaFason]([Recorrido_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



