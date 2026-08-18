CREATE TABLE [dbo].[LogIdentificacionVehicular]
(
    [Id]                            INT           IDENTITY(1,1) NOT NULL,
    [CodigoDispositivo]             NVARCHAR(50)  NOT NULL,
    [Tarjeta]                       NVARCHAR(50)  NULL,
    [ErrorDispositivo]                         NVARCHAR(500) NULL,
    [PuestoDeTrabajo_Id]            INT           NULL,
    [Recorrido_Id]                  INT           NULL,
    [Patente]                       NVARCHAR(20)  NULL,
    [VehiculoPresente]              BIT           NOT NULL,
    [FechaEvento]                   DATETIME      NOT NULL,
    [ResultadoWorkflow]             NVARCHAR(MAX) NULL,
    [PatenteLeida]                  NVARCHAR(20)  NULL,
    [DiferenciaSustitucion]         INT           NOT NULL DEFAULT 0,
    [DuracionMecanismoSustitucionMs]   INT           NULL,

    CONSTRAINT [PK_LogIdentificacionVehicular]
        PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_LogIdentificacionVehicular_PuestoDeTrabajo] 
        FOREIGN KEY ([PuestoDeTrabajo_Id])
        REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
        ON DELETE CASCADE,

    CONSTRAINT [FK_LogIdentificacionVehicular_Recorrido] 
        FOREIGN KEY ([Recorrido_Id])
        REFERENCES [dbo].[Recorrido] ([Id])
        ON DELETE CASCADE
);
GO