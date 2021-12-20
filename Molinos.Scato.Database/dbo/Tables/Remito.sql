CREATE TABLE [dbo].[Remito]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [OrdenDeDescarga] VARCHAR(15) NOT NULL UNIQUE, 
    [FechaOD] DATE NOT NULL,
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [TipoComercial_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.Remito_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    [CentroOrigen_Id] INT NULL,
	CONSTRAINT [FK_dbo.Remito_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroOrigen_Id]) REFERENCES [dbo].[Centro] ([Id]),  
    [ProveedorOrigen_Id] INT NULL,
	CONSTRAINT [FK_dbo.Remito_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([ProveedorOrigen_Id]) REFERENCES [dbo].[Proveedor] ([Id]),  
    [Transportista_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.Remito_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
    [Chofer_Id] INT NOT NULL,
	[OrdenRemito] VARCHAR(15) NOT NULL, 
    [Material_Id] INT NOT NULL, 
	[DocLegalRemito] VARCHAR(15) NULL, 
	[CodigoAnexo] VARCHAR(10) NULL, 
	[AcuerdoMarco] VARCHAR(15) NULL,
	[PesoTaraOrigen] INT NULL, 
    [PesoBrutoOrigen] INT NULL, 
    [PesoNetoOrigen] INT NULL,
	[Recorrido_Id] INT NULL,
    [KmRecorrer] INT NULL, 
    [Procedencia_Id] INT NULL, 
    [Cosecha] VARCHAR(5) NULL, 
    [CodEstab] VARCHAR(6) NULL,
	[EsExtranjero] BIT NULL,
    CONSTRAINT [FK_dbo.Remito_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.Remito_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Procedencia_Id]) REFERENCES [dbo].[Localidad] ([Id]),
	CONSTRAINT [FK_dbo.Remito_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Remito_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[Remito]([Recorrido_Id] ASC);