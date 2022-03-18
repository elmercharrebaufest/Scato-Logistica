CREATE TABLE [dbo].[Proveedor] (
    [Id]                     INT          IDENTITY (1, 1) NOT NULL,
    [Descripcion]            VARCHAR (50) NOT NULL,
    [RazonSocial]            VARCHAR (50) DEFAULT ('') NOT NULL,
    [CodigoSap]              VARCHAR (10) NULL,
    [Cuil]                   VARCHAR (13) NOT NULL,
    [Pais_Id]                INT          NULL,
    [Provincia_Id]           INT          NULL,
    [Localidad_Id]           INT          NULL,
    [Domicilio]              VARCHAR (35) NULL,
    [EsTitularCP]            BIT          DEFAULT ((0)) NOT NULL,
    [EsIntermediario]        BIT          DEFAULT ((0)) NOT NULL,
    [EsRemitenteComercial]   BIT          DEFAULT ((0)) NOT NULL,
    [EsDestinatario]         BIT          DEFAULT ((0)) NOT NULL,
    [PR]                     BIT          DEFAULT ((0)) NOT NULL,
    [CM]                     BIT          DEFAULT ((0)) NOT NULL,
    [AM]                     BIT          DEFAULT ((0)) NOT NULL,
    [VM]                     BIT          DEFAULT ((0)) NOT NULL,
    [ToleranciaEnPorcentaje] DECIMAL (18) NULL,
    [ToleranciaEnKg]         DECIMAL (18) NULL,
    [EnvioAutomaticoMail]    BIT          DEFAULT ((0)) NOT NULL,
    [Pesada]                 BIT          DEFAULT ((0)) NOT NULL,
    [Analisis]               BIT          DEFAULT ((0)) NOT NULL,
    [Mail]                   VARCHAR (45) NULL,
    [Activo]                 BIT          DEFAULT ((1)) NOT NULL,
    [EnvioCamaraDirecto]     BIT          DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Proveedor] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Proveedor_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Proveedor_dbo.Pais_Pais_Id] FOREIGN KEY ([Pais_Id]) REFERENCES [dbo].[Pais] ([Id]),
    CONSTRAINT [FK_dbo.Proveedor_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id]),
    CONSTRAINT [UK_Proveedor_CodigoSAP] UNIQUE NONCLUSTERED ([CodigoSap] ASC) WITH (FILLFACTOR = 90)
);



GO
CREATE NONCLUSTERED INDEX [IX_Pais_Id]
    ON [dbo].[Proveedor]([Pais_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Proveedor]([Localidad_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Proveedor]([Provincia_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);



