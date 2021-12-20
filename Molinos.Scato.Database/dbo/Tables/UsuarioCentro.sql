CREATE TABLE [dbo].[UsuarioCentro] (
    [Usuario_Id]      INT NOT NULL,
    [Centro_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.UsuarioCentro] PRIMARY KEY CLUSTERED ([Usuario_Id] ASC, [Centro_Id] ASC),
    CONSTRAINT [FK_dbo.UsuarioCentro_dbo.Usuario_Usuario_Id] FOREIGN KEY ([Usuario_Id]) REFERENCES [dbo].[Usuario] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UsuarioCentro_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Usuario_Id]
    ON [dbo].[UsuarioCentro]([Usuario_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[UsuarioCentro]([Centro_Id] ASC);

