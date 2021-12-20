CREATE TABLE [dbo].[PlanoDeCargaAgenteControlPrivadoHistorico] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [PlanoDeCargaHistorico_Id]					 INT             NOT NULL,
    [AgenteControlPrivado_Id]					 INT             NOT NULL,
    CONSTRAINT [PK_dbo.PlanoDeCargaAgenteControlPrivadoHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PlanoDeCargaAgenteControlPrivadoHistorico_dbo.PlanoDeCargaHistorico_PlanoDeCargaHistorico_Id] FOREIGN KEY ([PlanoDeCargaHistorico_Id]) REFERENCES [dbo].[PlanoDeCargaHistorico] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.PlanoDeCargaAgenteControlPrivadoHistorico_dbo.AgenteControlPrivado_AgenteControlPrivado_Id] FOREIGN KEY ([AgenteControlPrivado_Id]) REFERENCES [dbo].[AgenteControlPrivado] ([Id]),
);