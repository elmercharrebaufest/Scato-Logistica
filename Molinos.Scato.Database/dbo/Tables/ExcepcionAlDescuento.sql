CREATE TABLE [dbo].[ExcepcionAlDescuento] (
    [Id]						INT      IDENTITY (1, 1) NOT NULL,
    [FechaDesde]				DATETIME NOT NULL,
    [FechaHasta]				DATETIME NOT NULL,
	[Usuario]					NVARCHAR (40) NOT NULL,
    [Proveedor_Id]				INT      NOT NULL,
    [CaracteristicaDeCalidad_Id]      INT      NOT NULL,
    [Camara_Id]      INT      NOT NULL,
	[Motivo]					NVARCHAR(4000)      NOT NULL,
    
	CONSTRAINT [PK_dbo.ExcepcionAlDescuento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ExcepcionAlDescuento_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionAlDescuento_dbo.CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionAlDescuento_dbo.Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
);