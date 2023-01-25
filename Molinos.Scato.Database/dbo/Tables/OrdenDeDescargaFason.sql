CREATE TABLE [dbo].[OrdenDeDescargaFason]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Numero] VARCHAR(15) NOT NULL UNIQUE,
    [NumeroRemito] VARCHAR(15) NULL,
    [FechaOD] DATE NOT NULL,
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [TipoComercial_Id] INT NOT NULL,
	[EsExtranjero] BIT NULL,
    [NumeroCTG] VARCHAR(12) NULL,
    [Sucursal] VARCHAR(20) NULL,
    [NroOrden] VARCHAR(20) NULL

	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    [Material_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    [Procedencia_Id] INT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Procedencia_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    [Cliente_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Cliente_Cliente_Id] FOREIGN KEY ([Cliente_Id]) REFERENCES [dbo].[Cliente] ([Id]), 
    [Transportista_Id] INT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
    [Chofer_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    [PesoTaraOrigen] INT NULL, 
    [PesoBrutoOrigen] INT NULL, 
    [PesoNetoOrigen] INT NOT NULL,
	[Recorrido_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescargaFason_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenDeDescargaFason]([Recorrido_Id] ASC);