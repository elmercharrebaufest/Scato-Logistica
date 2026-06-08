CREATE TABLE [dbo].[LogIdentificacionVehicularDetalle]
(
    [Id]                                  INT             IDENTITY(1,1) NOT NULL,
    [LogIdentificacionVehicular_Id]       INT             NOT NULL,
    [ProveedorALPR]                       NVARCHAR(100)   NOT NULL,
    [CodigoCamara]                        NVARCHAR(100)   NOT NULL,
    [RutaImagen]                          NVARCHAR(500)   NULL,
    [Intentos]                            INT             NOT NULL,
    [Patente]                             NVARCHAR(20)    NULL,
    [Certeza]                             DECIMAL(5,2)    NULL,
    [Exitoso]                             BIT             NOT NULL,

    CONSTRAINT [PK_LogIdentificacionVehicularDetalle] 
        PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_LogIdentificacionVehicularDetalle_Log] 
        FOREIGN KEY ([LogIdentificacionVehicular_Id])
        REFERENCES [dbo].[LogIdentificacionVehicular] ([Id])
        ON DELETE CASCADE
);
GO