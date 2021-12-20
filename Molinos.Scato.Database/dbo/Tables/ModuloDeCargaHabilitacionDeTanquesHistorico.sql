CREATE TABLE [dbo].[ModuloDeCargaHabilitacionDeTanquesHistorico]
(
	[Id]                INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaHistorico_Id]  INT NOT NULL,
    [Tanque1]           BIT NOT NULL DEFAULT 0,
    [Tanque2]           BIT NOT NULL DEFAULT 0,
    [Tanque7]           BIT NOT NULL DEFAULT 0,
    [Tanque8]           BIT NOT NULL DEFAULT 0,
    [Tanque9]           BIT NOT NULL DEFAULT 0,
    [Tanque20]          BIT NOT NULL DEFAULT 0,
    [Tanque30]          BIT NOT NULL DEFAULT 0,
    [Tanque31]          BIT NOT NULL DEFAULT 0,
    [Tanque32]          BIT NOT NULL DEFAULT 0,
    [Tanque33]          BIT NOT NULL DEFAULT 0,
    [Tanque34]          BIT NOT NULL DEFAULT 0,
    [Tanque35]          BIT NOT NULL DEFAULT 0,
    [Tanque36]          BIT NOT NULL DEFAULT 0,
    [Tanque37]          BIT NOT NULL DEFAULT 0,
    [Tanque38]          BIT NOT NULL DEFAULT 0,
    [Tanque40]          BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_dbo.ModuloDeCargaHabilitacionDeTanquesHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaHabilitacionDeTanquesHistorico_dbo.ModuloDeCargaHistorico_ModuloDeCargaHistorico_Id] FOREIGN KEY ([ModuloDeCargaHistorico_Id]) REFERENCES [dbo].[ModuloDeCargaHistorico] ([Id]) on delete cascade
);