CREATE TABLE [dbo].[LecturaDeTarjeta] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[PuestoDeTrabajo_Id]          INT            NULL,
	[Lectura]      NVARCHAR (15) NOT NULL,
	[Patente]      NVARCHAR (15) NULL,
	[PatenteLeida]      NVARCHAR (15) NULL,
	[OcrActivo]      bit not null default 0,
	[ReconocimientoExitoso]      bit not null default 0,
    CONSTRAINT [PK_dbo.LecturaDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.LecturaDeTarjeta_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[LecturaDeTarjeta]([PuestoDeTrabajo_Id] ASC);