CREATE TABLE [dbo].[ModuloDeCargaManosDeEmbarqueHistorico]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaHistorico_Id] INT NOT NULL,
    [Mano]                      INT NOT NULL,
    [Observaciones]             NVARCHAR(500) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaManosDeEmbarqueHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarqueHistorico_dbo.ModuloDeCargaHistorico_ModuloDeCargaHistorico_Id] FOREIGN KEY ([ModuloDeCargaHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaHistorico] ([Id]) on delete cascade
);