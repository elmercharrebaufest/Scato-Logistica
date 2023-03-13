CREATE TABLE [dbo].[LecturaDeTarjeta] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [PuestoDeTrabajo_Id]    INT           NULL,
    [Lectura]               NVARCHAR (15) NOT NULL,
    [Patente]               NVARCHAR (15) NULL,
    [PatenteLeida]          NVARCHAR (15) NULL,
    [OcrActivo]             BIT           DEFAULT ((0)) NOT NULL,
    [ReconocimientoExitoso] BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.LecturaDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LecturaDeTarjeta_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[LecturaDeTarjeta]([PuestoDeTrabajo_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

