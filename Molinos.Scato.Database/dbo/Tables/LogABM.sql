CREATE TABLE [dbo].[LogABM] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Pantalla] NVARCHAR (100) NULL,
    [Usuario]  NVARCHAR (100) NULL,
    [Fecha]    DATETIME       NULL,
    [Evento]   INT            NULL,
    [Entidad]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.LogABM] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



