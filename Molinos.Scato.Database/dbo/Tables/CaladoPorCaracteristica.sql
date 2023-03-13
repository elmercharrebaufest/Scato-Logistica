CREATE TABLE [dbo].[CaladoPorCaracteristica] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [CaracteristicaDeCalidad_Id] INT             NOT NULL,
    [AnalisisPreliminar]         BIT             NOT NULL,
    [ValorCalado]                DECIMAL (18, 2) NULL,
    [DescuentoEnKg]              DECIMAL (18, 2) NULL,
    [DescuentoEnPorcentaje]      DECIMAL (18, 2) NULL,
    [Unidad]                     NVARCHAR (20)   NOT NULL,
    [Rango]                      NVARCHAR (20)   NULL,
    [Calado_Id]                  INT             NOT NULL,
    [HuboExcepcion]              BIT             DEFAULT ((0)) NOT NULL,
    [AnalisisAutomatico]         BIT             DEFAULT ((0)) NOT NULL,
    [EnviaACamara]               BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.CaladoPorCaracteristica] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CaladoPorCaracteristica_dbo.Calado_Calado_Id] FOREIGN KEY ([Calado_Id]) REFERENCES [dbo].[Calado] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CaladoPorCaracteristica_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Calado_Id]
    ON [dbo].[CaladoPorCaracteristica]([Calado_Id] ASC)
    INCLUDE([CaracteristicaDeCalidad_Id], [Id]) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_CaracteristicaDeCalidad_Id]
    ON [dbo].[CaladoPorCaracteristica]([CaracteristicaDeCalidad_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);


GO

