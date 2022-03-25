CREATE TABLE [dbo].[PlanoDeCargaAgenteControlPrivado] (
    [Id]                      INT IDENTITY (1, 1) NOT NULL,
    [PlanoDeCarga_Id]         INT NOT NULL,
    [AgenteControlPrivado_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.PlanoDeCargaAgenteControlPrivado] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PlanoDeCargaAgenteControlPrivado_dbo.AgenteControlPrivado_AgenteControlPrivado_Id] FOREIGN KEY ([AgenteControlPrivado_Id]) REFERENCES [dbo].[AgenteControlPrivado] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCargaAgenteControlPrivado_dbo.PlanoDeCarga_PlanoDeCarga_Id] FOREIGN KEY ([PlanoDeCarga_Id]) REFERENCES [dbo].[PlanoDeCarga] ([Id]) ON DELETE CASCADE
);


