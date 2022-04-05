CREATE TABLE [dbo].[CaracteristicasAnalizadas] (
    [Id]                                    INT             IDENTITY (1, 1) NOT NULL,
    [EsHumedad]                             BIT             DEFAULT ((0)) NOT NULL,
    [EsGranosVerdes]                        BIT             DEFAULT ((0)) NOT NULL,
    [EsGranosDañados]                       BIT             DEFAULT ((0)) NOT NULL,
    [EsCuerposExtranos]                     BIT             DEFAULT ((0)) NOT NULL,
    [EsSemillaSoja]                         BIT             DEFAULT ((0)) NOT NULL,
    [EsProteinaBaja]                        BIT             DEFAULT ((0)) NOT NULL,
    [EsProteinaAlta]                        BIT             DEFAULT ((0)) NOT NULL,
    [TieneDescuentos]                       BIT             DEFAULT ((0)) NOT NULL,
    [TieneInsectosVivos]                    BIT             DEFAULT ((0)) NOT NULL,
    [CaracteristicasNoCorrenspodenEspecial] BIT             DEFAULT ((0)) NOT NULL,
    [Humedad]                               DECIMAL (18, 2) NULL,
    [Grado]                                 DECIMAL (18, 2) NULL,
    [Recorrido_Id]                          INT             DEFAULT ((1)) NULL,
    [Calidad]                               INT             DEFAULT ((0)) NOT NULL,
    [EsProteinaMedia]                       BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.CaracteristicasAnalizadas] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CaracteristicasAnalizadas_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);


