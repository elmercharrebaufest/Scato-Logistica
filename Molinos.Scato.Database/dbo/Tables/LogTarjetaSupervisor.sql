CREATE TABLE [dbo].[LogTarjetaSupervisor] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [PuestoDeTrabajo_Id] INT            NOT NULL,
    [Fecha]              DATETIME       NOT NULL,
    [Motivo]             NVARCHAR (250) NULL,
    [NumeroTarjeta]      NVARCHAR (20)  NULL,
    [NombreUsuario]      NVARCHAR (20)  NULL,
    CONSTRAINT [PK_dbo.LogTarjetaSupervisor] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LogTarjetaSupervisor_dbo.LogTarjetaSupervisor_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);

