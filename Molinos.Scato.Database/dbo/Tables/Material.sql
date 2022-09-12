CREATE TABLE [dbo].[Material] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [CodigoSAP]                  NVARCHAR (20)			 NULL,
    [Descripcion]                NVARCHAR (40)   NOT NULL,
    [DescripcionCorta]           NVARCHAR (15)    NULL,
    [Tipo]                       NVARCHAR (20)   NULL,
    [CodigoEspecie]              INT             NULL,
    [UnidadDeMedidad]            NVARCHAR (3)    NULL,
    [FactorConversion]           DECIMAL (18, 2) NULL,
    [TipoDeGrano]                INT  NULL,
    [CodigoONCCA]                NVARCHAR (20)  NULL,
    [CodigoEstablecimientoONCCA] INT             NULL,
    [AnalisisInterno]            INT NULL,
    [RequiereNumeroTropa]        BIT             NOT NULL,
    [UsaBinPallet]               BIT             NOT NULL,
    [Peso]                       DECIMAL (18, 2) NULL,
	[Clase]						 INT   NULL,
    [EsUva]                      BIT             NOT NULL,
    [Variedad_Id]                INT             NULL,
    [Commodity]                  BIT             NOT NULL,
    [EsCosecha]                  BIT             NOT NULL,
    [AlmacenPredeterminado_Id]   INT             NULL,
    [AlmacenOrigen_Id]           INT             NULL,
    [Activo]                     BIT             NOT NULL DEFAULT 1,
	[RequiereAnexoInase]         BIT             NOT NULL DEFAULT 0,
    [Lote] BIT NOT NULL DEFAULT 0, 
    [Contrato] BIT NOT NULL DEFAULT 0, 
    [PesoTeoricoSap] DECIMAL(18, 2) NULL,
	[NirsCodigoProducto] NVARCHAR (20) NULL,
    [Oleico] BIT NOT NULL DEFAULT 0, 
    [EsGrano] BIT NOT NULL DEFAULT 0, 
	[VigenciaDesde]              INT        NULL,
    [VigenciaHasta]              INT        NULL,
    [EsInsumo] BIT NOT NULL DEFAULT 0, 
    [EsAsignableCalle] BIT NOT NULL DEFAULT 0, 
    [ColorFondo] NVARCHAR(7) NULL, 
    [ColorTexto] NVARCHAR(7) NULL, 
    CONSTRAINT [PK_dbo.Material] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Material_dbo.Almacen_AlmacenOrigen_Id] FOREIGN KEY ([AlmacenOrigen_Id]) REFERENCES [dbo].[Almacen] ([Id]),
    CONSTRAINT [FK_dbo.Material_dbo.Almacen_AlmacenPredeterminado_Id] FOREIGN KEY ([AlmacenPredeterminado_Id]) REFERENCES [dbo].[Almacen] ([Id]),
	CONSTRAINT [FK_dbo.Material_dbo.Variedad_Variedad_Id] FOREIGN KEY ([Variedad_Id]) REFERENCES [dbo].[Variedad] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_AlmacenPredeterminado_Id]
    ON [dbo].[Material]([AlmacenPredeterminado_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AlmacenOrigen_Id]
    ON [dbo].[Material]([AlmacenOrigen_Id] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Material_CodigoSAP]
    ON [dbo].[Material]([CodigoSAP] ASC)
	WHERE [CodigoSAP] IS NOT NULL
GO

