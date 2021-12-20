CREATE TABLE [dbo].[OrdenCargaInterna] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [NumeroOrden]      NVARCHAR (40) NOT NULL,
    [FechaEmision]     DATETIME       NOT NULL,
    [FechaCreacion]     DATETIME       NOT NULL DEFAULT GETDATE(),
    [PatenteCamion]    NVARCHAR (10) NOT NULL,
    [PatenteAcoplado]  NVARCHAR (10) NULL,
    [Transportista_Id] INT            NOT NULL,
    [TipoComercial_Id] INT            NOT NULL,
    [Material_Id]      INT            NOT NULL,
    [Destino_Id]       INT            NOT NULL,
    [Chofer_Id]        INT            NOT NULL,
    [Recorrido_Id]     INT            NOT NULL,
	[KmRecorrer] NVARCHAR (10) NULL, 
	[LocalidadDestino_Id] INT NULL,
	[EsExtranjero] BIT NULL,
    CONSTRAINT [PK_dbo.OrdenCargaInterna] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Centro_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
	CONSTRAINT [FK_dbo.OrdenCargaInterna_dbo.Localidad_Localidad_Id] FOREIGN KEY ([LocalidadDestino_Id]) REFERENCES [dbo].[Localidad] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Transportista_Id]
    ON [dbo].[OrdenCargaInterna]([Transportista_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TipoComercial_Id]
    ON [dbo].[OrdenCargaInterna]([TipoComercial_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[OrdenCargaInterna]([Material_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Destino_Id]
    ON [dbo].[OrdenCargaInterna]([Destino_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Chofer_Id]
    ON [dbo].[OrdenCargaInterna]([Chofer_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenCargaInterna]([Recorrido_Id] ASC);

