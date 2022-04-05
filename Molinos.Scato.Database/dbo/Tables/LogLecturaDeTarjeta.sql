CREATE TABLE [dbo].[LogLecturaDeTarjeta] (
    [Id]                       INT           IDENTITY (1, 1) NOT NULL,
    [Fecha]                    DATETIME      NOT NULL,
    [PuestoDeTrabajo_Id]       INT           NOT NULL,
    [Patente]                  NVARCHAR (15) NULL,
    [PatenteLeida]             NVARCHAR (15) NULL,
    [Tolerancia]               INT           NULL,
    [CantidadDeDiferencias]    INT           DEFAULT ((0)) NOT NULL,
    [ExisteOtroCamionEnPlanta] BIT           NULL,
    [ReconocimientoExitoso]    BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.LogLecturaDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LogLecturaDeTarjeta_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[LogLecturaDeTarjeta]([PuestoDeTrabajo_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);


