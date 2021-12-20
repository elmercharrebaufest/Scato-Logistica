CREATE TABLE [dbo].[CaracteristicasCartaPorteOtrosPuertos]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY, 
    [CartaPorteOtrosPuertos_Id] INT NOT NULL, 
    [CodigoExterno] NVARCHAR(50) NOT NULL, 
    [Valor] DECIMAL(15, 5) NOT NULL
)
