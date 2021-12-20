CREATE TABLE [dbo].[ModuloDeCargaHistorico]
(
	[Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Cargado]           BIT                           NOT NULL default 0,
    [ModuloDeCarga_Id]    INT            NOT NULL,
    [FechaDeCreacion] DATETIME NULL,
    [FechaDeModificacion] DATETIME NULL,
    [Usuario]    NVARCHAR(40)                          NULL,
    [FechaDeFinalizacion] DATETIME NULL,
    [UsuarioFinalizacion]    NVARCHAR(40)                          NULL,
    [Enviado]               BIT NOT NULL default 0,
    CONSTRAINT [PK_dbo.ModuloDeCargaHistorico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaHistorico_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id])
);