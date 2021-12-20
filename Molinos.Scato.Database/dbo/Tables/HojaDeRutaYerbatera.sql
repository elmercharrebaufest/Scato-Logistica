CREATE TABLE [dbo].[HojaDeRutaYerbatera]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [NroHojaDeRutaYerbatera] VARCHAR(13) NOT NULL, 
	[Proveedor_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),

	[Procedencia_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Procedencia_Id]) REFERENCES [dbo].[Localidad] ([Id]),

	[Destinatario_Id] INT NULL, 
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Destinatario_Proveedor_Id] FOREIGN KEY ([Destinatario_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    
	[CentroDestino_Id] INT NULL,
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroDestino_Id]) REFERENCES [dbo].[Centro] ([Id]),
    
	[Transportista_Id] INT NULL,
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
   
    [Chofer_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),

	[FechaCarga] DATETIME NOT NULL,
	[FechaVencimiento] DATETIME NOT NULL,
	[FechaEmision] DATETIME NOT NULL,

	[TipoComercial_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    
	[Material_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]), 
 
	[Patente] VARCHAR(12) NOT NULL, 
	[PatenteAcoplado] VARCHAR(12) NULL, 

	[PesoBrutoOrigen] INT NULL, 
	[PesoTaraOrigen] INT NULL, 
	[PesoNetoOrigen] INT NULL, 
	[Recorrido_Id]     INT            NOT NULL,
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.HojaDeRutaYerbatera_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,

)

GO
CREATE NONCLUSTERED INDEX [IX_HojaDeRutaYerbatera_NroHojaDeRutaYerbatera]
    ON [dbo].[HojaDeRutaYerbatera]([NroHojaDeRutaYerbatera] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[HojaDeRutaYerbatera]([Recorrido_Id] ASC);