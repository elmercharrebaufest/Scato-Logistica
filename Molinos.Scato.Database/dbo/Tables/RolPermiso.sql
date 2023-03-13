CREATE TABLE [dbo].[RolPermiso] (
    [Rol_Id]     INT NOT NULL,
    [Permiso_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.RolPermiso] PRIMARY KEY CLUSTERED ([Rol_Id] ASC, [Permiso_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.RolPermiso_dbo.Permiso_Permiso_Id] FOREIGN KEY ([Permiso_Id]) REFERENCES [dbo].[Permiso] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RolPermiso_dbo.Rol_Rol_Id] FOREIGN KEY ([Rol_Id]) REFERENCES [dbo].[Rol] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_Rol_Id]
    ON [dbo].[RolPermiso]([Rol_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Permiso_Id]
    ON [dbo].[RolPermiso]([Permiso_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);



