CREATE TABLE [dbo].[LoteBiotecnologia] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [NumeroDeLote]  NVARCHAR (20) NOT NULL,
    [NombreUsuario] NVARCHAR (20) NOT NULL,
    [Camara_Id]     INT           NOT NULL,
    [Material_Id]   INT           NOT NULL,
    [Fecha]         DATETIME      NOT NULL,
    [FechaDesde]    DATETIME      NOT NULL,
    [FechaHasta]    DATETIME      NOT NULL,
    [Centro_Id]     INT           NOT NULL,
    CONSTRAINT [PK_dbo.LoteBiotecnologia] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LoteBiotecnologia_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
    CONSTRAINT [FK_dbo.LoteBiotecnologia_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.LoteBiotecnologia_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id])
);

