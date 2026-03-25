CREATE TABLE [dbo].[PuestoDeTrabajo] (
    [Id]                        	INT IDENTITY (1, 1) NOT NULL,
    [NombrePuesto]               	NVARCHAR (35) NOT NULL,
    [Lector]               			NVARCHAR (35) NOT NULL,
    [Entrada]               		NVARCHAR (400) NULL,
    [EntradaSupervisor]             NVARCHAR (400) NOT NULL,
    [SensorQuiebre]               	NVARCHAR (35) NOT NULL,
	[VideoCamara]			   		NVARCHAR (50) NULL,
	[VideoCamaraDirectorio]	   		NVARCHAR (100) NULL,
    [NombrePc]               		NVARCHAR (30) NOT NULL,
	[Automatico]               		BIT          NOT NULL,
	[PidePatente]               	BIT          NOT NULL,
	[FotoAlMarcarTarjeta]           BIT          NOT NULL default 0,
	[ImprimeTarjetaDeAcceso]     	BIT          DEFAULT 0,
	[Centro_Id]	 					INT  NOT NULL,
    [EncolaLecturas] 				BIT NOT NULL DEFAULT 1,
	[InvisibleEnListaDeTareas]      BIT          NOT NULL default 0,
    [Automatizado] 					BIT NOT NULL DEFAULT 0, 
	[SinAfip] 						BIT NOT NULL DEFAULT 0, 
	[SinCupo] 						BIT NOT NULL DEFAULT 0, 
	[SinFotoCartaPorte] 			BIT NOT NULL DEFAULT 0,
	[ImprimeCartaPorte] 			BIT NOT NULL DEFAULT 0,
    [LectorQr] 						NVARCHAR(35) NULL,
	[CartelLed] 					NVARCHAR(35) NULL,
    [Balanza_Id] 					INT NULL,
	[AutomatizadoFull] 				BIT NOT NULL DEFAULT 0, 
	[NoAsignaCalleEnGaritaEntrada] 	BIT NOT NULL DEFAULT 1,
	[PausaAutoFull] 				BIT NOT NULL DEFAULT 0, 
	[Firmware] 						NVARCHAR(300)  NULL,
	[CierreSupervisor]      		NVARCHAR (400) NULL,
    [CierreEntrada]					NVARCHAR (400)  NULL, 
    [Concentrador] 					NVARCHAR(100) NULL, 
    [OrdenBalanza] 					INT NULL, 
    [IntercomunicadorCodigo] 		NVARCHAR(50) NULL, 
	[ActivarRegistroInactividad] 	BIT NOT NULL DEFAULT 0,
	[SemaforoRojo] 					NVARCHAR(100) NULL,
	[SemaforoAmarillo] 				NVARCHAR(100) NULL,
	[SemaforoVerde] 				NVARCHAR(100) NULL,
	[ConfigSensores_Id] 			INT NULL, 
	[VisualizacionBarrera_Id]		INT NULL,
	[RequiereComentarioGestionarBarrera] 	BIT NOT NULL DEFAULT 0,
	[GrupoBarreraCodigo] 		NVARCHAR(50) NULL, 
	[SensorVehicular] 			NVARCHAR(35) NULL,
    CONSTRAINT [PK_dbo.PuestoDeTrabajo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.Balanza_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.ConfigSensor_Id] FOREIGN KEY ([ConfigSensores_Id]) REFERENCES [dbo].[ConfigSensores] ([Id]),
	CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.VisualizacionBarrera_Id] FOREIGN KEY ([VisualizacionBarrera_Id]) REFERENCES [dbo].[VisualizacionBarrera] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[PuestoDeTrabajo]([Centro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Lector]
    ON [dbo].[PuestoDeTrabajo]([Lector] ASC) WHERE ([Lector] IS NOT NULL) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_NombrePc]
    ON [dbo].[PuestoDeTrabajo]([NombrePc] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Balanza]
    ON [dbo].[PuestoDeTrabajo]([Balanza_Id] ASC) WHERE ([Balanza_Id] IS NOT NULL) WITH (STATISTICS_NORECOMPUTE = ON);

