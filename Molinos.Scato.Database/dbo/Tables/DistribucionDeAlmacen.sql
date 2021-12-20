CREATE TABLE [dbo].[DistribucionDeAlmacen] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [DistribucionDeAlmacenes_Id] INT            NULL,
    [Almacen_Id] INT            NULL,
	[Litros] INT            NULL,
    CONSTRAINT [PK_dbo.DistribucionDeAlmacen] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DistribucionDeAlmacen_dbo.DistribucionDeAlmacenes_DistribucionDeAlmacenes_Id] FOREIGN KEY ([DistribucionDeAlmacenes_Id]) REFERENCES [dbo].[DistribucionDeAlmacenes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.DistribucionDeAlmacen_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Almacen_Id]
    ON [dbo].[DistribucionDeAlmacen]([Almacen_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DistribucionDeAlmacenes_Id]
    ON [dbo].[DistribucionDeAlmacen]([DistribucionDeAlmacenes_Id] ASC);


