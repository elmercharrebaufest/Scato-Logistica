CREATE TABLE [dbo].[ConfiguracionGeneral] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.ConfiguracionGeneral] PRIMARY KEY CLUSTERED ([Id] ASC), 
    [Pantalla] NVARCHAR(100) NOT NULL, 
    [Nombre] NVARCHAR(100) NOT NULL,
    [Valor] NVARCHAR(MAX) NOT NULL,
    [Centro_Id] INT NULL, 
    [FechaCreacion] DATETIME NOT NULL, 
    [UsuarioCreacion] NVARCHAR(100) NOT NULL, 
    [FechaUltimaModificacion] DATETIME NULL, 
    [UsuarioUltimaModificacion] NVARCHAR(100) NULL,

    CONSTRAINT [FK_dbo.ConfiguracionGeneral_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
);
