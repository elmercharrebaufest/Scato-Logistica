CREATE TABLE [dbo].[ModuloDeCargaHabilitacionDeTanquesHistorico] (
    [Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaHistorico_Id] INT NOT NULL,
    [Tanque1]                   BIT DEFAULT ((0)) NOT NULL,
    [Tanque2]                   BIT DEFAULT ((0)) NOT NULL,
    [Tanque7]                   BIT DEFAULT ((0)) NOT NULL,
    [Tanque8]                   BIT DEFAULT ((0)) NOT NULL,
    [Tanque9]                   BIT DEFAULT ((0)) NOT NULL,
    [Tanque20]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque30]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque31]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque32]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque33]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque34]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque35]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque36]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque37]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque38]                  BIT DEFAULT ((0)) NOT NULL,
    [Tanque40]                  BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaHabilitacionDeTanquesHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ModuloDeCargaHabilitacionDeTanquesHistorico_dbo.ModuloDeCargaHistorico_ModuloDeCargaHistorico_Id] FOREIGN KEY ([ModuloDeCargaHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaHistorico] ([Id]) ON DELETE CASCADE
);

