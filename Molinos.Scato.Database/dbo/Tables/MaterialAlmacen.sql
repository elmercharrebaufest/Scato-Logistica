CREATE TABLE [dbo].[MaterialAlmacen] (
    [Material_Id]      INT NOT NULL,
    [Almacen_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.MaterialAlmacen] PRIMARY KEY CLUSTERED ([Material_Id] ASC, [Almacen_Id] ASC),
    CONSTRAINT [FK_dbo.MaterialAlmacen_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MaterialAlmacen_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[MaterialAlmacen]([Material_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Almacen_Id]
    ON [dbo].[MaterialAlmacen]([Almacen_Id] ASC);

