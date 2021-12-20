CREATE TABLE [dbo].[ModuloDeCargaUmap]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
	[FechaEncendido]			datetime NULL,
	[HoraEncendido]				nvarchar(8) NULL,
	[FechaApagado]				datetime NULL,
	[HoraApagado]				nvarchar(8) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaUmap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaUmap_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade
);