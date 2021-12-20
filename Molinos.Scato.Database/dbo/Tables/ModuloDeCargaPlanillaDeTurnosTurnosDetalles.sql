CREATE TABLE [dbo].[ModuloDeCargaPlanillaDeTurnosTurnosDetalles]
(
	[Id]                                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaPlanillaDeTurnosTurnos_Id]    INT NOT NULL,
    [Exportador_Id]                             INT NOT NULL,
    [Linea]                                     NVARCHAR (10),
    [BodegaParcel]                              INT,
    [MaterialPuerto_Id]                         INT NOT NULL,
    [Tk]                                        NVARCHAR (10),
    [Temperatura]                               FLOAT,
    [MedidaInicialCM]                           FLOAT,
    [MedidaInicialMM]                           FLOAT,
    [MedidaFinalCM]                             FLOAT,
    [MedidaFinalMM]                             FLOAT,
    [Destino_Id]                                INT NOT NULL,
    [Cantidad]                                  INT,
    CONSTRAINT [PK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosDetalles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosDetalles_dbo.ModuloDeCargaPlanillaDeTurnosTurnos_MDCPlanillaDeTurnosTurnos_Id] FOREIGN KEY ([ModuloDeCargaPlanillaDeTurnosTurnos_Id]) REFERENCES [dbo].[ModuloDeCargaPlanillaDeTurnosTurnos] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosDetalles_dbo.Exportador_Exportador_Id] FOREIGN KEY ([Exportador_Id]) REFERENCES [dbo].[Exportador] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosDetalles_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosDetalles_dbo.Destino_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Destino] ([Id])
);