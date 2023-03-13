CREATE TABLE [dbo].[MicroMuestrasPorCasillero] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [Muestra_Id]   INT      NOT NULL,
    [Casillero_Id] INT      NOT NULL,
    [Fecha]        DATETIME NOT NULL,
    CONSTRAINT [PK_MicroMuestrasPorCasillero] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_MicroMuestrasPorCasillero_Casillero] FOREIGN KEY ([Casillero_Id]) REFERENCES [dbo].[Casillero] ([Id]),
    CONSTRAINT [FK_MicroMuestrasPorCasillero_MuestraEnvioACamara] FOREIGN KEY ([Muestra_Id]) REFERENCES [dbo].[MuestraEnvioACamara] ([Id]) ON DELETE CASCADE
);


