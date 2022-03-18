CREATE TABLE [dbo].[IngresoDeDatosDeExportacion] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [PermisoEmbarque]         NVARCHAR (50) NULL,
    [IdentificadorContenedor] NVARCHAR (50) NULL,
    [Firma_Id]                INT           NULL,
    [Recorrido_Id]            INT           NOT NULL,
    [Nacionalidad_Id]         INT           NOT NULL,
    [Transportista_Id]        INT           NOT NULL,
    [PesoNeto]                INT           DEFAULT ((0)) NULL,
    CONSTRAINT [PK_dbo.IngresoDeDatosDeExportacion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.IngresoDeDatosDeExportacion_dbo.Firma_Firma_Id] FOREIGN KEY ([Firma_Id]) REFERENCES [dbo].[Firma] ([Id]),
    CONSTRAINT [FK_dbo.IngresoDeDatosDeExportacion_dbo.Nacionalidad_Nacionalidad_Id] FOREIGN KEY ([Nacionalidad_Id]) REFERENCES [dbo].[Pais] ([Id]),
    CONSTRAINT [FK_dbo.IngresoDeDatosDeExportacion_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.IngresoDeDatosDeExportacion_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id])
);


