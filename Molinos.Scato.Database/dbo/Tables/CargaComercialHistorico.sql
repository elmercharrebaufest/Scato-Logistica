CREATE TABLE [dbo].[CargaComercialHistorico] (
    [Id]                       INT IDENTITY (1, 1) NOT NULL,
    [Cantidad]                 INT NOT NULL,
    [Exportador_Id]            INT NOT NULL,
    [MaterialPuerto_Id]        INT NOT NULL,
    [PlanoDeCargaHistorico_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.CargaComercialHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CargaComercialHistorico_dbo.Exportador_Exportador_Id] FOREIGN KEY ([Exportador_Id]) REFERENCES [dbo].[Exportador] ([Id]),
    CONSTRAINT [FK_dbo.CargaComercialHistorico_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.CargaComercialHistorico_dbo.PlanoDeCargaHistorico_PlanoDeCargaHistorico_Id] FOREIGN KEY ([PlanoDeCargaHistorico_Id]) REFERENCES [dbo].[PlanoDeCargaHistorico] ([Id]) ON DELETE CASCADE
);


