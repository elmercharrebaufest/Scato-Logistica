CREATE TABLE [dbo].[ModuloDeCargaManosDeEmbarqueDetalleHistorico]
(
	[Id]                                            INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaManosDeEmbarqueHistorico_Id]      INT NOT NULL,
    [CeldaManoDeEmbarque_Id]                        INT NULL,
    [SentidoManoDeEmbarque_Id]                      INT NULL,
    [PorcentajePorMano]                             INT NULL,
    [AperturaPorton]                                BIT NULL,
    [MasProduccion]                                 BIT NULL,
    [MaterialPuerto_Id] INT NULL, 
    CONSTRAINT [PK_dbo.ModuloDeCargaManosDeEmbarqueDetalleHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalleHistorico_dbo.ModuloDeCargaManosDeEmbarqueHist_ModuloDeCargaManosDeEmbarqueHist_Id] FOREIGN KEY ([ModuloDeCargaManosDeEmbarqueHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaManosDeEmbarqueHistorico] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalleHistorico_dbo.CeldaManoDeEmbarque_CeldaManoDeEmbarque_Id] FOREIGN KEY ([CeldaManoDeEmbarque_Id]) REFERENCES [dbo].[CeldaManoDeEmbarque] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalleHistorico_dbo.SentidoManoDeEmbarque_SentidoManoDeEmbarque_Id] FOREIGN KEY ([SentidoManoDeEmbarque_Id]) REFERENCES [dbo].[SentidoManoDeEmbarque] ([Id]),
        CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueDetalleHistorico_dbo.SentidoManoDeEmbarque_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id])
);