CREATE TABLE [dbo].[ModuloDeCargaRitmosEmbarque]
(
	[Id]                                INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]                  INT NOT NULL,
    [FechaHoraArranqueBalanza7]         DATETIME NOT NULL,
    [UltimaBalanzadaBalanza7]           DATETIME NOT NULL,
    [tnTotalesBalanza7]                 INT NOT NULL default 0,
    [FechaHoraArranqueBalanza8]         DATETIME NOT NULL,
    [UltimaBalanzadaBalanza8]           DATETIME NOT NULL,
    [tnTotalesBalanza8]                 INT NOT NULL default 0,
    CONSTRAINT [PK_dbo.ModuloDeCargaRitmosEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaRitmosEmbarque_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade
);