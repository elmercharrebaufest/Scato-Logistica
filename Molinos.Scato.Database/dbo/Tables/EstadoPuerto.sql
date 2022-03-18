CREATE TABLE [dbo].[EstadoPuerto] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [FechaCalado]    DATETIME      NULL,
    [FechaUbicacion] DATETIME      NULL,
    [FechaAlturaRio] DATETIME      NULL,
    [Calado]         NVARCHAR (20) NULL,
    [Ubicacion]      NVARCHAR (20) NULL,
    [AlturaDelRio]   NVARCHAR (20) NULL,
    CONSTRAINT [PK_dbo.EstadoPuerto] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON)
);

