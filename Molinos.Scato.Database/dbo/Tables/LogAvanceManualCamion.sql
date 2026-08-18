CREATE TABLE [dbo].[LogAvanceManualCamion]
(
    [Id]                                   INT            IDENTITY(1,1) NOT NULL,
    [NombreUsuario]                        NVARCHAR(MAX)  NULL,
    [FechaEvento]                          DATETIME       NOT NULL,
    [PuestoDeTrabajo_Id]                   INT            NULL,
    [MotivoFallo]                          NVARCHAR(MAX)  NULL,
    [PatenteLeida]                         NVARCHAR(MAX)  NULL,
    [PatenteIngresada]                     NVARCHAR(MAX)  NULL,
    [Tarjeta]                              NVARCHAR(MAX)  NULL,
    [Atendido]                             INT            NOT NULL CONSTRAINT [DF_LogAvanceManualCamion_Atendido] DEFAULT (0),
    [FechaAtencion]                        DATETIME       NULL,
    [LogIdentificacionVehicularId_origen]  INT            NULL,
    [LogIdentificacionVehicularId_Destino] INT            NULL,
    [MotivoLiberar]                        NVARCHAR(MAX)  NULL,

    CONSTRAINT [PK_dbo.LogAvanceManualCamion]
        PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_dbo.LogAvanceManualCamion_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id]
        FOREIGN KEY ([PuestoDeTrabajo_Id])
        REFERENCES [dbo].[PuestoDeTrabajo] ([Id]),

    CONSTRAINT [FK_dbo.LogAvanceManualCamion_dbo.LogIdentificacionVehicular_Origen]
        FOREIGN KEY ([LogIdentificacionVehicularId_origen])
        REFERENCES [dbo].[LogIdentificacionVehicular] ([Id]),

    CONSTRAINT [FK_dbo.LogAvanceManualCamion_dbo.LogIdentificacionVehicular_Destino]
        FOREIGN KEY ([LogIdentificacionVehicularId_Destino])
        REFERENCES [dbo].[LogIdentificacionVehicular] ([Id])
);
GO
