CREATE TABLE [dbo].[MaterialPorCentro] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [AnalisisInterno]            INT             NULL,
    [AlmacenPredeterminado_Id]   INT             NULL,
    [Centro_Id]                  INT             NOT NULL,
    [Material_Id]                  INT             NOT NULL,
	[Camara_Id]                  INT             NULL,
    [CorrespondeDescarga] BIT NOT NULL DEFAULT 0, 
	[RequiereTecnologia] BIT NOT NULL DEFAULT 0, 
	[MaterialDeTerceros] BIT NOT NULL DEFAULT 0, 
    [PorcentajeMuestraAuditoria] DECIMAL(18, 2) NULL, 
    [ImprimeReciboMunicipal] BIT NOT NULL DEFAULT 0, 
    [NoValidaCG] BIT NOT NULL DEFAULT 0,
	[EpaStockPorCorte] INT NULL,
	[MostrarEnWebMobile] BIT NOT NULL DEFAULT 0,
	[DescripcionWebMobile] NVARCHAR (40) NULL,
    [Orden] INT NULL DEFAULT 0, 
    [IgnoraContingencia] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.MaterialPorCentro] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MaterialPorCentro_dbo.Almacen_AlmacenPredeterminado_Id] FOREIGN KEY ([AlmacenPredeterminado_Id]) REFERENCES [dbo].[Almacen] ([Id]),
    CONSTRAINT [FK_dbo.MaterialPorCentro_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.MaterialPorCentro_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [UK_dbo.MaterialPorCentro] UNIQUE ([Material_Id], [Centro_Id]),
	CONSTRAINT [FK_dbo.Material_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_AlmacenPredeterminado_Id]
    ON [dbo].[MaterialPorCentro]([AlmacenPredeterminado_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[MaterialPorCentro]([Centro_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[MaterialPorCentro]([Material_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Camara_Id]
    ON [dbo].[MaterialPorCentro]([Camara_Id] ASC);