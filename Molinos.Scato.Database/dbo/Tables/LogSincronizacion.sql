CREATE TABLE [dbo].[LogSincronizacion]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [NombreInterface] NVARCHAR(50) NOT NULL, 
    [FechaEjecucion] DATETIME NOT NULL,
    [Completa] BIT NOT NULL DEFAULT 0,
    [Correcta] BIT NOT NULL DEFAULT 0,
	[Mensaje] NVARCHAR(MAX) NULL
)
