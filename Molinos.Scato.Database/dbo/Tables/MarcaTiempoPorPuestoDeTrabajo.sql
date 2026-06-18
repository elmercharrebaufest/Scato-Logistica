CREATE TABLE [dbo].[MarcaTiempoPorPuestoDeTrabajo] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [PuestoDeTrabajoId]   INT              NULL,
    [FechaInicio]         DATETIME         NULL,
    [FechaIdentificacion] DATETIME         NULL,
    [FechaFin]            DATETIME         NULL,
    [TipoIdentificacion]  INT              NULL,
    [RecorridoId]         INT              NULL,
    CONSTRAINT [PK_MarcaTiempoPorPuestoDeTrabajo] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_MarcaTiempoPorPuestoDeTrabajo_PuestoDeTrabajoId] 
        FOREIGN KEY ([PuestoDeTrabajoId]) 
        REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_MarcaTiempoPorPuestoDeTrabajo_RecorridoId] 
        FOREIGN KEY ([RecorridoId])
        REFERENCES [dbo].[Recorrido] ([Id])
        ON DELETE CASCADE
);
GO
