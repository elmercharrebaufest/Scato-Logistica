CREATE TABLE [dbo].[EquivalenciasCaracteristicasCpOtrosPuertos]
(
	[Id] INT IDENTITY NOT NULL PRIMARY KEY, 
    [Material_Id] INT NOT NULL, 
    [CodigoExterno] NVARCHAR(50) NOT NULL, 
    [CodigoSap] NVARCHAR(20) NOT NULL, 
    [EsHumedad] BIT NOT NULL DEFAULT 0 
)
