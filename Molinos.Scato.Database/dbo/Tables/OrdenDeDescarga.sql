CREATE TABLE [dbo].[OrdenDeDescarga]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Numero] VARCHAR(15) NOT NULL UNIQUE, 
    [FechaMovimiento] DATE NOT NULL,
	[PatenteCamion] VARCHAR(10) NOT NULL,
	[PatenteAcoplado] VARCHAR(10) NULL,
    [TipoComercial_Id] INT NOT NULL,
	[EsExtranjero] BIT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescarga_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    [Proveedor_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.OrdenDeDescarga_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]), 
    [Transportista_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescarga_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),   
    [Chofer_Id] INT NOT NULL,
    CONSTRAINT [FK_dbo.OrdenDeDescarga_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
	[Recorrido_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.OrdenDeDescarga_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[OrdenDeDescarga]([Recorrido_Id] ASC);