CREATE TABLE [dbo].[LogDispositivo]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Fecha] DATETIME NOT NULL, 
    [PuestoDeTrabajo_Id] INT NOT NULL, 
    [CodigoDispositivo] NVARCHAR(50) NOT NULL, 
    [NombreLog] NVARCHAR(50) NOT NULL, 
    [Valor] NVARCHAR(200) NULL,
)
