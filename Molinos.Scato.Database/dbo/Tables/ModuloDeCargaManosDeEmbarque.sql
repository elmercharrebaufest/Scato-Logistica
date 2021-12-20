CREATE TABLE [dbo].[ModuloDeCargaManosDeEmbarque]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
    [Mano]                      INT NOT NULL,
    [Observaciones]             NVARCHAR(500) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaManosDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaManosDeEmbarque_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade,
);
