CREATE TABLE [dbo].[SensorBarrera]
(
	[Id]								INT IDENTITY (1, 1) NOT NULL,
	[CodigoDispositivoSensorArriba]		NVARCHAR (100)  NULL,
	[CodigoDispositivoSensorAbajo]		NVARCHAR (100)  NULL,	
	[VisualizacionBarrera_Id]			INT NOT NULL,
	[CodigoDispositivoSensorQuiebre]	NVARCHAR (100)  NULL,
	[Barrera]							NVARCHAR (100)  NULL,
	[BarreraBajar]						NVARCHAR (100)  NULL

	CONSTRAINT [PK_dbo.SensorBarrera] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.SensorBarrera_dbo.VisualizacionBarrera_VisualizacionBarrera_Id] FOREIGN KEY ([VisualizacionBarrera_Id]) REFERENCES [dbo].[VisualizacionBarrera] ([Id])
);
