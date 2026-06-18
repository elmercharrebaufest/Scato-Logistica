CREATE TABLE [dbo].[SensorMarcaTiempoPorPuestoDeTrabajo] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [CodigoSensor]      NVARCHAR(50) NOT NULL,
    [PuestoDeTrabajoId] INT NOT NULL,
    [TipoSensor]        INT NOT NULL DEFAULT(1),
    CONSTRAINT [PK_dbo.SensorMarcaTiempoPorPuestoDeTrabajo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.SensorMarcaTiempoPorPuestoDeTrabajo_dbo.PuestoDeTrabajo_PuestoDeTrabajoId]
        FOREIGN KEY ([PuestoDeTrabajoId]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SensorMarcaTiempoPorPuestoDeTrabajo_CodigoSensor]
    ON [dbo].[SensorMarcaTiempoPorPuestoDeTrabajo]([CodigoSensor] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_SensorMarcaTiempoPorPuestoDeTrabajo_PuestoDeTrabajoId]
    ON [dbo].[SensorMarcaTiempoPorPuestoDeTrabajo]([PuestoDeTrabajoId] ASC);
