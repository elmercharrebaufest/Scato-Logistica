CREATE TABLE [dbo].[RegistroStockOtrosPuertos]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[CodigoEstablecimiento]         NVARCHAR(10) NOT NULL,
	[Cosecha]         NVARCHAR (10) NOT NULL,
	[PesoNeto]    DECIMAL(18, 2) NOT NULL,
	[CartaPorteOtrosPuertos_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.RegistroStockOtrosPuertos_dbo.CartaPorteOtrosPuertos_Id] FOREIGN KEY ([CartaPorteOtrosPuertos_Id]) REFERENCES [dbo].CartaPorteOtrosPuertos ([Id])
)
