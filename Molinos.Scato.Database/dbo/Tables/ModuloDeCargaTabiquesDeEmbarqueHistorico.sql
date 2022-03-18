CREATE TABLE [dbo].[ModuloDeCargaTabiquesDeEmbarqueHistorico] (
    [Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaHistorico_Id] INT NOT NULL,
    [Tabique]                   INT NOT NULL,
    [EntreColumna]              INT NULL,
    [YColumna]                  INT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaTabiquesDeEmbarqueHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ModuloDeCargaTabiquesDeEmbarqueHistorico_dbo.ModuloDeCargaHistorico_ModuloDeCargaHistorico_Id] FOREIGN KEY ([ModuloDeCargaHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaHistorico] ([Id]) ON DELETE CASCADE
);

