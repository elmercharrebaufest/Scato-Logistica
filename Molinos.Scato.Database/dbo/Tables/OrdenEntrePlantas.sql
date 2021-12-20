CREATE TABLE [dbo].[OrdenEntrePlantas]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Numero] VARCHAR(15) NOT NULL UNIQUE, 
    [Fecha] DATETIME NOT NULL,
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [TipoComercial_Id] INT NOT NULL,
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    [Material_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    [CentroDestino_Id] INT NULL,
	CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroDestino_Id]) REFERENCES [dbo].[Centro] ([Id]), 
    [Transportista_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
    [Chofer_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    [Recorrido_Id] INT NOT NULL,
    [CodigoAnexo] NVARCHAR(10) NULL, 
    [KmRecorrer] INT NULL, 
    CONSTRAINT [FK_dbo.OrdenEntrePlantas_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenEntrePlantas]([Recorrido_Id] ASC);