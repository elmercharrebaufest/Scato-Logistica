CREATE TABLE [dbo].[OrdenDeCargaContenedor]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [NroOrdenDeCargaContenedor] VARCHAR(15) NOT NULL UNIQUE, 
    [Fecha] DATE NOT NULL,
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [TipoComercial_Id] INT NOT NULL, 
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    [Destino_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeCargaContenedor.Centro_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    [Transportista_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
    [Chofer_Id] INT NOT NULL,
    [Material_Id] INT NOT NULL, 
	[Recorrido_Id] INT NOT NULL,
    CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    [ContenedorEntrada_Id] INT NOT NULL, 
    [ContenedorSalida_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.TaraContenedor_ContenedorEntrada_Id] FOREIGN KEY ([ContenedorEntrada_Id]) REFERENCES [dbo].[TaraContenedor] ([Id]),
	CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.TaraContenedor_ContenedorSalida_Id] FOREIGN KEY ([ContenedorSalida_Id]) REFERENCES [dbo].[TaraContenedor] ([Id]),
    CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.OrdenDeCargaContenedor_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id] ON [dbo].[OrdenDeCargaContenedor]([Recorrido_Id] ASC);