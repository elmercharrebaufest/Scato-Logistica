CREATE TABLE [dbo].[ModuloDeCargaPlanillaDeTurnosTurnosCortes]
(
	[Id]                                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaPlanillaDeTurnosTurnos_Id]    INT NOT NULL,
    [MotivosDeCorte_Id]                         INT NOT NULL,
    [HoraInicio]                                NVARCHAR (8),
    [HoraFin]                                   NVARCHAR (8),
    [TiempoTotal]                               NVARCHAR (8),
    [Observaciones]                             NVARCHAR (500),
    CONSTRAINT [PK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosCortes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosCortes_dbo.ModuloDeCargaPlanillaDeTurnosTurnos_MDCPlanillaDeTurnosTurnos_Id] FOREIGN KEY ([ModuloDeCargaPlanillaDeTurnosTurnos_Id]) REFERENCES [dbo].[ModuloDeCargaPlanillaDeTurnosTurnos] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnosCortes_dbo.MotivosDeCorte_MotivosDeCorte_Id] FOREIGN KEY ([MotivosDeCorte_Id]) REFERENCES [dbo].[MotivosDeCorte] ([Id])
);