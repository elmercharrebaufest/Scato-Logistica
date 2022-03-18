CREATE TABLE [dbo].[ModuloDeCargaManosDeEmbarqueDetalle] (
    [Id]                              INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaManosDeEmbarque_Id] INT NOT NULL,
    [CeldaManoDeEmbarque_Id]          INT NULL,
    [SentidoManoDeEmbarque_Id]        INT NULL,
    [PorcentajePorMano]               INT NULL,
    [AperturaPorton]                  BIT NULL,
    [MasProduccion]                   BIT NULL,
    [MaterialPuerto_Id]               INT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaManosDeEmbarqueDetalle] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalle_dbo.CeldaManoDeEmbarque_CeldaManoDeEmbarque_Id] FOREIGN KEY ([CeldaManoDeEmbarque_Id]) REFERENCES [dbo].[CeldaManoDeEmbarque] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalle_dbo.ModuloDeCargaManosDeEmbarque_ModuloDeCargaManosDeEmbarque_Id] FOREIGN KEY ([ModuloDeCargaManosDeEmbarque_Id]) REFERENCES [dbo].[ModuloDeCargaManosDeEmbarque] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalle_dbo.SentidoManoDeEmbarque_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalle_dbo.SentidoManoDeEmbarque_SentidoManoDeEmbarque_Id] FOREIGN KEY ([SentidoManoDeEmbarque_Id]) REFERENCES [dbo].[SentidoManoDeEmbarque] ([Id])
);

