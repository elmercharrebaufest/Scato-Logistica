CREATE TABLE [dbo].[CaracteristicaDeCalidad] (
    [Id]                                INT             IDENTITY (1, 1) NOT NULL,
    [CaracteristicaDeCalidadMaestro_Id] INT             NULL,
    [Descripcion]                       NVARCHAR (30)   NULL,
    [DescripcionCorta]                  NVARCHAR (8)    NOT NULL,
    [UnidadDeMedida]                    NVARCHAR (3)    NOT NULL,
    [DescuentoEnPorcentaje]             INT             NOT NULL,
    [CaladoMinimo]                      DECIMAL (18, 2) NOT NULL,
    [CaladoMaximo]                      DECIMAL (18, 2) NOT NULL,
    [CodigoSAP]                         NVARCHAR (20)   NOT NULL,
    [Analisis]                          INT             NOT NULL,
    [Ensayo]                            NVARCHAR (10)   NULL,
    [Obligatorio]                       BIT             DEFAULT ((0)) NOT NULL,
    [CargaEnCalado]                     BIT             NOT NULL,
    [NoObservableEnCalado]              BIT             NOT NULL,
    [InternoPorObservados]              BIT             NOT NULL,
    [InspeccionDeCamionesVacios]        BIT             NOT NULL,
    [EsModificable]                     BIT             NOT NULL,
    [EsHumedad]                         BIT             NOT NULL,
    [Material_Id]                       INT             NULL,
    [MaterialPorCentro_Id]              INT             NULL,
    [SituacionEnvioACamara]             INT             DEFAULT ((2)) NOT NULL,
    [EsTenorAzucarino]                  BIT             DEFAULT ((0)) NOT NULL,
    [EsEstadoSanitario]                 BIT             DEFAULT ((0)) NOT NULL,
    [EsCalidadUva]                      BIT             DEFAULT ((0)) NOT NULL,
    [EsCuerposExtranos]                 BIT             DEFAULT ((0)) NOT NULL,
    [EsGranosVerdes]                    BIT             DEFAULT ((0)) NOT NULL,
    [PrioridadEnCalado]                 INT             NULL,
    [NoAceptarSiSeDefineUnValor]        BIT             DEFAULT ((0)) NOT NULL,
    [EsGranosDañados]                   BIT             DEFAULT ((0)) NOT NULL,
    [ToleranciaSinAnalisis]             DECIMAL (18, 2) NULL,
    [EsMermaVolatil]                    BIT             DEFAULT ((0)) NOT NULL,
    [EsProteina]                        BIT             DEFAULT ((0)) NOT NULL,
    [ValorProteina]                     DECIMAL (18, 2) NULL,
    [ValorEspecialMinimo]               DECIMAL (18, 2) NULL,
    [ValorEspecialMaximo]               DECIMAL (18, 2) NULL,
    [EsAutomatizable]                   BIT             DEFAULT ((0)) NOT NULL,
    [Dispositivo]                       INT             DEFAULT ((0)) NOT NULL,
    [NombreNirs]                        NVARCHAR (50)   NULL,
    [EsInsectosVivos]                   BIT             DEFAULT ((0)) NOT NULL,
    [IntervaloDeAnalisis]               BIT             DEFAULT ((0)) NULL,
    [EnviaASap]                         BIT             DEFAULT ((1)) NOT NULL,
    [ToleranciaSinMensaje]              DECIMAL (18, 2) NULL,
    [SiSuperaValorCamara]               DECIMAL (18, 2) NULL,
    [ValorProteinaMedia]                DECIMAL (18, 2) NULL,
    [CaladoPorDefecto]                  DECIMAL(18, 2) NULL, 
    [EsPesoHectolitrico]                BIT NULL , 
    CONSTRAINT [PK_dbo.CaracteristicaDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CaracteristicaDeCalidad_dbo.CaracteristicaDeCalidadMaestro_CaracteristicaDeCalidadMaestro_Id] FOREIGN KEY ([CaracteristicaDeCalidadMaestro_Id]) REFERENCES [dbo].[CaracteristicaDeCalidadMaestro] ([Id]),
    CONSTRAINT [FK_dbo.CaracteristicaDeCalidad_dbo.MaterialPorCentro_MaterialPorCentro_Id] FOREIGN KEY ([MaterialPorCentro_Id]) REFERENCES [dbo].[MaterialPorCentro] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_MaterialPorCentro_Id]
    ON [dbo].[CaracteristicaDeCalidad]([MaterialPorCentro_Id] ASC);
GO

GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[CaracteristicasAnalizadas]([Recorrido_Id] ASC)
    INCLUDE([Id], [EsHumedad], [EsGranosVerdes], [EsGranosDañados], [EsCuerposExtranos], [EsProteinaBaja], [EsProteinaAlta], [TieneDescuentos], [Humedad]) WITH (STATISTICS_NORECOMPUTE = ON);


GO