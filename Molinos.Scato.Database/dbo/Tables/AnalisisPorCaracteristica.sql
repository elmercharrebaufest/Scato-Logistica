CREATE TABLE [dbo].[AnalisisPorCaracteristica] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [CaracteristicaDeCalidad_Id] INT             NOT NULL,
    [ValorCalado]                DECIMAL (18, 2) NULL,
    [ValorAnalisis]              DECIMAL (18, 2) NULL,
    [DescuentoEnKg]              DECIMAL (18, 2) NULL,
    [DescuentoEnPorcentaje]      DECIMAL (18, 2) NULL,
    [Unidad]                     NVARCHAR (20)   NOT NULL,
    [Rango]                      NVARCHAR (20)   NULL,
    [AnalisisDeCalidad_Id]       INT             NOT NULL,
    [HuboExcepcion]              BIT             DEFAULT ((0)) NOT NULL,
    [EnviaACamara]               BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.AnalisisPorCaracteristica] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AnalisisPorCaracteristica_dbo.AnalisisDeCalidad_AnalisisDeCalidad_Id] FOREIGN KEY ([AnalisisDeCalidad_Id]) REFERENCES [dbo].[AnalisisDeCalidad] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AnalisisPorCaracteristica_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Calado_Id]
    ON [dbo].[AnalisisPorCaracteristica]([AnalisisDeCalidad_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_CaracteristicaDeCalidad_Id]
    ON [dbo].[AnalisisPorCaracteristica]([CaracteristicaDeCalidad_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


