CREATE TABLE [dbo].[ModuloDeCargaTabiquesDeEmbarque]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
    [Tabique]                   INT NOT NULL,
    [EntreColumna]              INT NULL,
    [YColumna]                  INT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaTabiquesDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaTabiquesDeEmbarque_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade
);
