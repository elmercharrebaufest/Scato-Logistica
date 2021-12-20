CREATE TABLE [dbo].[Entregador]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [DescripcionCorta] VARCHAR(8) NOT NULL, 
    [Tratamiento] VARCHAR(10) NULL, 
    [RazonSocial] VARCHAR(35) NOT NULL, 
    [CodigoSAPCondicionFiscal] VARCHAR(10) NOT NULL, 
	[Cuil] VARCHAR(13) NOT NULL, 
	[Pais_Id] INT NULL,
	[Provincia_Id] INT NULL,
	[Localidad_Id] INT NULL,
	[Domicilio] VARCHAR(35) NULL, 
	[TipoEntregador] VARCHAR(8) NOT NULL, 
	[ToleranciaEnPorcentaje] DECIMAL NULL,
	[ToleranciaEnKg] DECIMAL NULL,
	[EnvioAutomaticoMail] BIT NOT NULL,
	[Pesada] BIT NOT NULL,
	[Analisis] BIT NOT NULL,
	[Mail] VARCHAR(45) NULL, 
	[Activo] BIT NOT NULL DEFAULT 1, 
	CONSTRAINT [PK_dbo.Entregador] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Entregador_dbo.Pais_Pais_Id] FOREIGN KEY ([Pais_Id]) REFERENCES [dbo].[Pais] ([Id]),
    CONSTRAINT [FK_dbo.Entregador_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Entregador_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id])
)

GO
CREATE NONCLUSTERED INDEX [IX_Pais_Id]
    ON [dbo].[Entregador]([Pais_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Entregador]([Localidad_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Entregador]([Provincia_Id] ASC);

