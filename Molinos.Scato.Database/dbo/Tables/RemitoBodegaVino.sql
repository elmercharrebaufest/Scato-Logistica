CREATE TABLE [dbo].[RemitoBodegaVino]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Patente] VARCHAR(10) NOT NULL,
	[NroRemito] NVARCHAR(13) NOT NULL,
	[OrdenDeCompra] NVARCHAR(20) NULL,
	[Proveedor_Id] INT NULL,
	[Transportista_Id] INT NOT NULL,
	[Chofer_Id] INT NOT NULL,
	[MarcaCamion] VARCHAR(30) NOT NULL,
	[ModeloCamion] NVARCHAR(4) NOT NULL,
	[TipoVehiculoBodega_Id] INT NOT NULL,
	[TipoComercial_Id] INT NOT NULL,
	[Material_Id] INT NOT NULL,
	[PesoBrutoOrigen] INT NULL,
	[PesoTaraOrigen] INT NULL,
	[PesoNetoOrigen] INT NULL,
	[Recorrido_Id] INT NULL,
	[Posicion] VARCHAR(30) NULL,
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.TipoVehiculoBodega_TipoVehiculoBodega_Id] FOREIGN KEY ([TipoVehiculoBodega_Id]) REFERENCES [dbo].[TipoVehiculoBodega] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]), 
	CONSTRAINT [FK_dbo.RemitoBodegaVino_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE, 
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[RemitoBodegaVino]([Recorrido_Id] ASC);