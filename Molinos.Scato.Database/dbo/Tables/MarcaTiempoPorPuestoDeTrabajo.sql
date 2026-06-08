CREATE TABLE [dbo].[MarcaTiempoPorPuestoDeTrabajo] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [NumeroDocumento]    NVARCHAR(50)     NULL,
    [PuestoDeTrabajoId]  INT              NOT NULL,
    [FechaInicio]        DATETIME         NULL,
    [FechaFin]           DATETIME         NULL,
    [Centro_Id]          INT              NULL,
    [TipoIngreso]        INT              NOT NULL DEFAULT(1),
    CONSTRAINT [PK_MarcaTiempoPorPuestoDeTrabajo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_MarcaTiempoPorPuestoDeTrabajo_Documento_Puesto] UNIQUE NONCLUSTERED ([NumeroDocumento] ASC, [PuestoDeTrabajoId] ASC),
    CONSTRAINT [FK_MarcaTiempoPorPuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajoId]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);
GO
