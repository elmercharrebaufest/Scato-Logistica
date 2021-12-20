CREATE TABLE [dbo].[CaracteristicasAnalizadas]
(
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
	EsHumedad BIT NOT NULL  DEFAULT 0, 
	EsGranosVerdes BIT NOT NULL  DEFAULT 0, 
	EsGranosDañados BIT NOT NULL  DEFAULT 0, 
	EsCuerposExtranos BIT NOT NULL DEFAULT 0, 
    EsSemillaSoja BIT NOT NULL DEFAULT 0, 
	EsProteinaBaja BIT NOT NULL DEFAULT 0,
	EsProteinaAlta BIT NOT NULL DEFAULT 0,
	TieneDescuentos BIT NOT NULL DEFAULT 0,
	TieneInsectosVivos BIT NOT NULL DEFAULT 0,
	CaracteristicasNoCorrenspodenEspecial BIT NOT NULL DEFAULT 0,	
	Humedad DECIMAL(18, 2) NULL,
	Grado DECIMAL(18, 2) NULL,
	[Recorrido_Id]          INT            NULL default 1,
	[Calidad]          INT           NOT NULL default 0,
    CONSTRAINT [PK_dbo.CaracteristicasAnalizadas] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CaracteristicasAnalizadas_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
)
