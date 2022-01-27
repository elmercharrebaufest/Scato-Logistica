CREATE TABLE [dbo].[EntidadTipoDeActividad] (
    [Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Entidad_Id] INT NOT NULL REFERENCES [dbo].[Entidad] ([Id]),
    [TipoDeActividad_Id] INT NOT NULL REFERENCES [dbo].[TipoDeActividad] ([Id])
);

