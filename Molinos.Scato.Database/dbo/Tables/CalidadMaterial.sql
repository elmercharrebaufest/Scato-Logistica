CREATE TABLE [dbo].[CalidadMaterial] (
    [Id]			INT IDENTITY (1, 1) NOT NULL,
    [TieneAnalisis] BIT NOT NULL,
	[MaterialPorCentro_Id]	INT NOT NULL,
	[Descripcion]	NVARCHAR(50) NOT NULL,
	[ValorDesde]	DECIMAL(18,2) NOT NULL,
	[ValorHasta]	DECIMAL(18,2) NOT NULL,
	CONSTRAINT [PK_dbo.CalidadMaterial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT	[FK_dbo.CalidadMaterial.MaterialPorCentro_MaterialPorCentro_Id] FOREIGN KEY ([MaterialPorCentro_Id]) REFERENCES [dbo].[MaterialPorCentro] ([Id]),
);