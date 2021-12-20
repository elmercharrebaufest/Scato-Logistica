CREATE TABLE [dbo].[HojaDeRuta]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Numero] VARCHAR(15) NOT NULL UNIQUE, 
    [Transportista_Id] INT NOT NULL,
    [Chofer_Id] INT NOT NULL,
    [TipoComercial_Id] INT NOT NULL, 
    [Material_Id] INT NOT NULL, 
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [Recorrido_Id] INT NOT NULL,
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.HojaDeRuta_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
	CONSTRAINT [FK_dbo.HojaDeRuta_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.HojaDeRuta_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
	CONSTRAINT [FK_dbo.HojaDeRuta_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.HojaDeRuta_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
);

GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[HojaDeRuta]([Recorrido_Id] ASC);