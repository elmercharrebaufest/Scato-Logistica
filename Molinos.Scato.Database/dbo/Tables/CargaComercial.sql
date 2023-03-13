CREATE TABLE [dbo].[CargaComercial] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [Cantidad]          INT NOT NULL,
    [Exportador_Id]     INT NOT NULL,
    [MaterialPuerto_Id] INT NOT NULL,
    [PlanoDeCarga_Id]   INT NOT NULL,
    CONSTRAINT [PK_dbo.CargaComercial] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CargaComercial_dbo.Exportador_Exportador_Id] FOREIGN KEY ([Exportador_Id]) REFERENCES [dbo].[Exportador] ([Id]),
    CONSTRAINT [FK_dbo.CargaComercial_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.CargaComercial_dbo.PlanoDeCarga_PlanoDeCarga_Id] FOREIGN KEY ([PlanoDeCarga_Id]) REFERENCES [dbo].[PlanoDeCarga] ([Id]) ON DELETE CASCADE
);


