CREATE TABLE [dbo].[ModuloDeCargaPlanillaDeTurnos]
(
	[Id]                INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]  INT NOT NULL,
	[Fecha]				datetime,
    CONSTRAINT [PK_dbo.ModuloDeCargaPlanillaDeTurnos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnos_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade,
);