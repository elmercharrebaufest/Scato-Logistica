CREATE TABLE [dbo].[ModuloDeCargaMangueraCarga]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
	[FechaConexionMangueras]	datetime NULL,
	[HoraConexionMangueras]		nvarchar(8) NULL,
	[FechaDesconexionMangueras]	datetime NULL,
	[HoraDesconexionMangueras]	nvarchar(8) NULL,
	[FechaComienzoCarga]		datetime NULL,
	[HoraComienzoCarga]			nvarchar(8) NULL,
	[FechaFinalizacionCarga]	datetime NULL,
	[HoraFinalizacionCarga]		nvarchar(8) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaMangueraCarga] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaMangueraCarga_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade
);