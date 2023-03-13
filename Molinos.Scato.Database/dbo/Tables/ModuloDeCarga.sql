CREATE TABLE [dbo].[ModuloDeCarga] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Cargado]             BIT           DEFAULT ((0)) NOT NULL,
    [FechaDeCreacion]     DATETIME      NULL,
    [FechaDeModificacion] DATETIME      NULL,
    [Usuario]             NVARCHAR (40) NULL,
    [FechaDeFinalizacion] DATETIME      NULL,
    [UsuarioFinalizacion] NVARCHAR (40) NULL,
    [Enviado]             BIT           DEFAULT ((0)) NOT NULL,
    [IniciarCarga]        BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.ModuloDeCarga] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON)
);

