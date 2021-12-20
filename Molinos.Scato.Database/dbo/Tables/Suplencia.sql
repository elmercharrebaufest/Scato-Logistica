CREATE TABLE [dbo].[Suplencia] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [UsuarioASuplantar_Id]            INT             NOT NULL,
    [UsuarioSuplente_Id]              INT             NOT NULL,
    [FechaDesde]                      DATE            NOT NULL, 
    [FechaHasta]                      DATE            NOT NULL, 

	CONSTRAINT [PK_dbo.Suplencia] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Suplencia_dbo.Usuario_UsuarioASuplantar_Id] FOREIGN KEY ([UsuarioASuplantar_Id]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_dbo.Suplencia_dbo.Usuario_UsuarioSuplente_Id] FOREIGN KEY ([UsuarioSuplente_Id]) REFERENCES [dbo].[Usuario] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_UsuarioASuplantar_Id]
    ON [dbo].[Suplencia]([UsuarioASuplantar_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UsuarioSuplente_Id]
    ON [dbo].[Suplencia]([UsuarioSuplente_Id] ASC);
