CREATE TABLE [dbo].[ModuloDeCarga]
(
	[Id]                    INT IDENTITY (1, 1) NOT NULL,
    [Cargado]               BIT NOT NULL default 0,
    [FechaDeCreacion]       DATETIME NULL,
    [FechaDeModificacion]   DATETIME NULL,
    [Usuario]               NVARCHAR(40) NULL,
    [FechaDeFinalizacion]   DATETIME NULL,
    [UsuarioFinalizacion]   NVARCHAR(40) NULL,
    [Enviado]               BIT NOT NULL default 0,
    [IniciarCarga]          BIT NOT NULL default 0,
    CONSTRAINT [PK_dbo.ModuloDeCarga] PRIMARY KEY CLUSTERED ([Id] ASC)
);