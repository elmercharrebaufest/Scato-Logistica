CREATE TABLE [dbo].[ModuloDeCargaPlanillaDeTurnosTurnos]
(
	[Id]                                INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCargaPlanillaDeTurnos_Id]  INT NOT NULL,
    [TurnoPuerto_Id]                    INT NOT NULL,
    [Cerrado]                           BIT NOT NULL default 0,
    [Enviado]                           BIT NOT NULL default 0,
    CONSTRAINT [PK_dbo.ModuloDeCargaPlanillaDeTurnosTurnos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnos_dbo.ModuloDeCargaPlanillaDeTurnos_ModuloDeCargaPlanillaDeTurnos_Id] FOREIGN KEY ([ModuloDeCargaPlanillaDeTurnos_Id]) REFERENCES [dbo].[ModuloDeCargaPlanillaDeTurnos] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaPlanillaDeTurnosTurnos_dbo.TurnoPuerto_TurnoPuerto_Id] FOREIGN KEY ([TurnoPuerto_Id]) REFERENCES [dbo].[TurnoPuerto] ([Id]),
);