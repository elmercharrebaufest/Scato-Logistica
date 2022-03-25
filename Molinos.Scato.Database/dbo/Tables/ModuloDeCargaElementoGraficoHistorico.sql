CREATE TABLE [dbo].[ModuloDeCargaElementoGraficoHistorico] (
    [Id]                        INT           IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaHistorico_Id] INT           NOT NULL,
    [CeldaManoDeEmbarque_Id]    INT           NULL,
    [Tipo]                      NVARCHAR (15) NULL,
    [X]                         FLOAT (53)    NULL,
    [Y]                         FLOAT (53)    NULL,
    [Forma]                     NVARCHAR (15) NULL,
    [Width]                     FLOAT (53)    NULL,
    [Height]                    FLOAT (53)    NULL,
    [RadioX]                    FLOAT (53)    NULL,
    [RadioY]                    FLOAT (53)    NULL,
    [MaterialPuerto_Id]         INT           NULL,
    [Rotacion]                  BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaElementoGraficoHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGraficoHistorico_dbo.CeldaManoDeEmbarque_CeldaManoDeEmbarque_Id] FOREIGN KEY ([CeldaManoDeEmbarque_Id]) REFERENCES [dbo].[CeldaManoDeEmbarque] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGraficoHistorico_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGraficoHistorico_dbo.ModuloDeCargaHistorico_ModuloDeCargaHistorico_Id] FOREIGN KEY ([ModuloDeCargaHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaHistorico] ([Id]) ON DELETE CASCADE
);

