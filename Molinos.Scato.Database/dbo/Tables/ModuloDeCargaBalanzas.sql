CREATE TABLE [dbo].[ModuloDeCargaBalanzas]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
	[MotivosFallasBalanza_Id]   INT NOT NULL,
    [Observaciones]             NVARCHAR (500),

    CONSTRAINT [PK_dbo.ModuloDeCargaBalanzas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaBalanzas_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaBalanzas_dbo.MotivosFallasBalanza_MotivosFallasBalanza_Id] FOREIGN KEY ([MotivosFallasBalanza_Id]) REFERENCES [dbo].[MotivosFallasBalanza] ([Id])
);