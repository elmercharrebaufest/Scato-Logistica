CREATE TABLE [dbo].[PlanoDeCargaBodegaHistorico] (
    [Id]                       INT           IDENTITY (1, 1) NOT NULL,
    [Cantidad]                 INT           NOT NULL,
    [Condicion]                NVARCHAR (50) NULL,
    [SfFull]                   NVARCHAR (50) NULL,
    [MaterialPuerto_Id]        INT           NULL,
    [Destino_Id]               INT           NULL,
    [PlanoDeCargaHistorico_Id] INT           NOT NULL,
    [TanqueDeAbordo]           NVARCHAR (50) NULL,
    [BodegaParcel]             INT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_dbo.PlanoDeCargaBodegaHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodegaHistorico_dbo.Destino_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Destino] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodegaHistorico_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodegaHistorico_dbo.PlanoDeCargaHistorico_PlanoDeCargaHistorico_Id] FOREIGN KEY ([PlanoDeCargaHistorico_Id]) REFERENCES [dbo].[PlanoDeCargaHistorico] ([Id]) ON DELETE CASCADE
);


