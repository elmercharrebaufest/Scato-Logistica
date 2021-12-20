CREATE TABLE [dbo].[LogLecturaDeTarjeta]
(
	[Id] INT  IDENTITY (1, 1) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [PuestoDeTrabajo_Id] INT NOT NULL, 
    [Patente] NVARCHAR(15) NULL, 
    [PatenteLeida] NVARCHAR(15) NULL,
	[Tolerancia] INT NULL, 
	[CantidadDeDiferencias] INT NOT NULL default 0, 
	[ExisteOtroCamionEnPlanta] bit NULL, 
	[ReconocimientoExitoso]      bit not null default 0,
CONSTRAINT [PK_dbo.LogLecturaDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.LogLecturaDeTarjeta_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[LogLecturaDeTarjeta]([PuestoDeTrabajo_Id] ASC);
