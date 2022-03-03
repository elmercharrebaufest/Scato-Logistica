CREATE TABLE [dbo].[CaracteristicaDeCalidad]
(
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
	[CaracteristicaDeCalidadMaestro_Id]	INT  NULL,
	[Descripcion]        NVARCHAR (30)  NULL,
	[DescripcionCorta]	 NVARCHAR (8)	NOT NULL,
	[UnidadDeMedida]	NVARCHAR (3)    NOT NULL,
    [DescuentoEnPorcentaje] INT NOT NULL,
    [CaladoMinimo] DECIMAL(18, 2) NOT NULL, 
    [CaladoMaximo] DECIMAL(18, 2) NOT NULL,
	[CodigoSAP]          NVARCHAR (20)	NOT NULL,
    [Analisis] INT NOT NULL, 
    [Ensayo] NVARCHAR(10) NULL, 
    [Obligatorio] BIT NOT NULL DEFAULT 0, 
    [CargaEnCalado] BIT NOT NULL, 
    [NoObservableEnCalado] BIT NOT NULL, 
    [InternoPorObservados] BIT NOT NULL, 
    [InspeccionDeCamionesVacios] BIT NOT NULL, 
    [EsModificable] BIT NOT NULL,
	[EsHumedad] BIT NOT NULL, 
    [Material_Id] INT NULL,
	[MaterialPorCentro_Id] INT NULL,
    [SituacionEnvioACamara] INT NOT NULL DEFAULT 2,
	[EsTenorAzucarino] BIT NOT NULL  DEFAULT 0, 
	[EsEstadoSanitario] BIT NOT NULL  DEFAULT 0, 
	[EsCalidadUva] BIT NOT NULL  DEFAULT 0, 
	[EsCuerposExtranos] BIT NOT NULL DEFAULT 0, 
    [EsGranosVerdes] BIT NOT NULL DEFAULT 0, 
	[PrioridadEnCalado]        INT              NULL,
	[NoAceptarSiSeDefineUnValor] BIT NOT NULL DEFAULT 0, 
	[EsGranosDañados] BIT NOT NULL DEFAULT 0,
    [ToleranciaSinAnalisis] DECIMAL(18, 2) NULL,
	[EsMermaVolatil] BIT NOT NULL DEFAULT 0,
	[EsProteina] BIT NOT NULL DEFAULT 0,
	[ValorProteina] DECIMAL(18, 2) NULL,
    [ValorProteinaMedia] DECIMAL(18, 2) NULL,
    [ValorEspecialMinimo] DECIMAL(18, 2) NULL, 
    [ValorEspecialMaximo] DECIMAL(18, 2) NULL,
    [EsAutomatizable] BIT NOT NULL DEFAULT 0, 
    [Dispositivo] INT NOT NULL DEFAULT 0,
	[NombreNirs] NVARCHAR(50) null,
	[EsInsectosVivos] BIT NOT NULL DEFAULT 0,
    [IntervaloDeAnalisis] BIT NULL DEFAULT 0, 
    [EnviaASap] BIT NOT NULL DEFAULT 1, 
    [ToleranciaSinMensaje] DECIMAL(18, 2) NULL,
	[SiSuperaValorCamara] DECIMAL(18, 2) NULL, 
    [CaladoPorDefecto] DECIMAL(18, 2) NULL, 
    CONSTRAINT [PK_dbo.CaracteristicaDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CaracteristicaDeCalidad_dbo.MaterialPorCentro_MaterialPorCentro_Id] FOREIGN KEY ([MaterialPorCentro_Id]) REFERENCES [dbo].[MaterialPorCentro] ([Id]),
	CONSTRAINT [FK_dbo.CaracteristicaDeCalidad_dbo.CaracteristicaDeCalidadMaestro_CaracteristicaDeCalidadMaestro_Id] FOREIGN KEY ([CaracteristicaDeCalidadMaestro_Id]) REFERENCES [dbo].[CaracteristicaDeCalidadMaestro] ([Id])
)

GO
CREATE NONCLUSTERED INDEX [IX_MaterialPorCentro_Id]
    ON [dbo].[CaracteristicaDeCalidad]([MaterialPorCentro_Id] ASC);
GO

GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
ON [dbo].[CaracteristicasAnalizadas] ([Recorrido_Id])
INCLUDE ([Id],[EsHumedad],[EsGranosVerdes],[EsGranosDañados],[EsCuerposExtranos],[EsProteinaBaja],[EsProteinaAlta],[TieneDescuentos],[Humedad])
GO