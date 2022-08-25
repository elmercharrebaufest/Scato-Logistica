CREATE TABLE [dbo].[LogDispositivo]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Fecha] DATETIME NOT NULL, 
    [CodigoDispositivo] NVARCHAR(50) NOT NULL, 
    [NombreLog] NVARCHAR(50) NOT NULL, 
    [ValorAnterior] NVARCHAR(200) NULL,
    [ValorActual] NVARCHAR(200) NULL, 
    CONSTRAINT [PK_dbo.LogDispositivo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
)
