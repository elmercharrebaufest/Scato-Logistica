CREATE TABLE [dbo].[PlanoDeCargaBodega] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Cantidad]          INT           NOT NULL,
    [Condicion]         NVARCHAR (50) NULL,
    [SfFull]            NVARCHAR (50) NULL,
    [MaterialPuerto_Id] INT           NULL,
    [Destino_Id]        INT           NULL,
    [PlanoDeCarga_Id]   INT           NOT NULL,
    [TanqueDeAbordo]    NVARCHAR (50) NULL,
    [BodegaParcel]      INT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_dbo.PlanoDeCargaBodega] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodega_dbo.Destino_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Destino] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodega_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCargaBodega_dbo.PlanoDeCarga_PlanoDeCarga_Id] FOREIGN KEY ([PlanoDeCarga_Id]) REFERENCES [dbo].[PlanoDeCarga] ([Id]) ON DELETE CASCADE
);


