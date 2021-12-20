CREATE TABLE [dbo].[EstadoConexion] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[PuestoDeTrabajo_Id]          INT            NOT NULL,
	[Estado]      BIT NOT NULL,
	[Mensaje]      NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.EstadoConexion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.EstadoConexion_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[EstadoConexion]([PuestoDeTrabajo_Id] ASC);