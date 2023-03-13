CREATE TABLE [dbo].[ConfigSensores] (
    [Id]                            INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]                   NVARCHAR (100)  NOT NULL,
    [SensorBarreraEntradaArriba]    NVARCHAR (100)  NOT NULL,
    [SensorBarreraEntradaAbajo]     NVARCHAR (100)  NOT NULL,
    [SensorPosicionIngreso]         NVARCHAR (100)  NOT NULL,
    [SensorPosicionSalida]          NVARCHAR (100)  NOT NULL,
    [SensorBarreraSalidaArriba]     NVARCHAR (100)  NOT NULL,
    [SensorBarreraSalidaAbajo]      NVARCHAR (100)  NOT NULL,
	[Centro_Id]                     INT NOT NULL,
    CONSTRAINT [PK_dbo.ConfigSensores] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ConfigSensores_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);
