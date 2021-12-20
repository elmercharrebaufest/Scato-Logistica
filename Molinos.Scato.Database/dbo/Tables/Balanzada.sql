CREATE TABLE [dbo].[Balanzada] (
    [Id]     INT NOT NULL,
	[NumeroBalanza] NVARCHAR(50) NOT NULL, 
    [PesoBruto] INT NULL, 
    [PesoNeto] INT NULL, 
	[PesoTara] INT NULL, 
	[Capacidad] NVARCHAR(30) NULL, 
	[ErrorSap] NVARCHAR(300) NULL, 
	[CargaInicial_Id] INT NULL, 
	[CargaInicial_NumeroBalanza] NVARCHAR(50) NULL, 

    [ModuloDeCargaBalanzas_Id] INT NULL, 
    CONSTRAINT [PK_dbo.Balanzada] PRIMARY KEY CLUSTERED ([Id] ASC,[NumeroBalanza]),
	CONSTRAINT [FK_dbo.Balanzada_dbo.Carga_CargaInicial_Id] FOREIGN KEY ([CargaInicial_Id],[CargaInicial_NumeroBalanza]) REFERENCES [dbo].[Carga] ([Id],[NumeroBalanza]),
	CONSTRAINT [FK_dbo.Balanzada_dbo.ModuloDeCargaBalanzas_ModuloDeCargaBalanzas_Id] FOREIGN KEY ([ModuloDeCargaBalanzas_Id]) REFERENCES [dbo].[ModuloDeCargaBalanzas] ([Id]),
);
GO

CREATE NONCLUSTERED INDEX IX_Balanzada_CargaInicial_IdCargaInicial_NumeroBalanza
ON [dbo].[Balanzada] ([CargaInicial_Id],[CargaInicial_NumeroBalanza])
INCLUDE ([PesoNeto])
GO