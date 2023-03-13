CREATE TABLE [dbo].[EntidadTipoDeActividad] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [Entidad_Id]         INT NOT NULL,
    [TipoDeActividad_Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    FOREIGN KEY ([Entidad_Id]) REFERENCES [dbo].[Entidad] ([Id]),
    FOREIGN KEY ([TipoDeActividad_Id]) REFERENCES [dbo].[TipoDeActividad] ([Id])
);



