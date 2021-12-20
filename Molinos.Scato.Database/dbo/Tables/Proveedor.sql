CREATE TABLE [dbo].[Proveedor] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion] VARCHAR(50) NOT NULL, 
    [RazonSocial] VARCHAR(50) NOT NULL DEFAULT '', 
    [CodigoSap] VARCHAR(10) NULL, 
	[Cuil] VARCHAR(13) NOT NULL, 
	[Pais_Id] INT NULL,
	[Provincia_Id] INT NULL,
	[Localidad_Id] INT NULL,
	[Domicilio] VARCHAR(35) NULL, 
	[EsTitularCP] BIT NOT NULL DEFAULT 0,
	[EsIntermediario] BIT NOT NULL DEFAULT 0,
	[EsRemitenteComercial] BIT NOT NULL DEFAULT 0,
	[EsDestinatario] BIT NOT NULL DEFAULT 0,
	[PR] BIT NOT NULL DEFAULT 0,
	[CM] BIT NOT NULL DEFAULT 0,
	[AM] BIT NOT NULL DEFAULT 0,
	[VM] BIT NOT NULL DEFAULT 0,
	[ToleranciaEnPorcentaje] DECIMAL NULL,
	[ToleranciaEnKg] DECIMAL NULL,
	[EnvioAutomaticoMail] BIT NOT NULL DEFAULT 0,
	[Pesada] BIT NOT NULL DEFAULT 0,
	[Analisis] BIT NOT NULL DEFAULT 0,
	[Mail] VARCHAR(45) NULL,
	[Activo] BIT NOT NULL DEFAULT 1,
	[EnvioCamaraDirecto] BIT NOT NULL DEFAULT 0,

	CONSTRAINT [PK_dbo.Proveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Proveedor_dbo.Pais_Pais_Id] FOREIGN KEY ([Pais_Id]) REFERENCES [dbo].[Pais] ([Id]),
    CONSTRAINT [FK_dbo.Proveedor_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Proveedor_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id]),
	CONSTRAINT UK_Proveedor_CodigoSAP UNIQUE (CodigoSap)
)

GO
CREATE NONCLUSTERED INDEX [IX_Pais_Id]
    ON [dbo].[Proveedor]([Pais_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Proveedor]([Localidad_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Proveedor]([Provincia_Id] ASC);

