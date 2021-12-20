CREATE TABLE [dbo].[DistribucionDeAlmacenes] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id]         INT NOT NULL,
    [NombreDeUsuario]  NVARCHAR (50) NOT NULL,
	[Fecha]               DATETIME       NOT NULL,

    CONSTRAINT [PK_dbo.DistribucionDeAlmacenes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DistribucionDeAlmacenes_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[DistribucionDeAlmacenes]([Recorrido_Id] ASC);

