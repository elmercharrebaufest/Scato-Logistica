CREATE TABLE [dbo].[MotivosDeCorte] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]	NVARCHAR(60)    NOT NULL,
    [Siglas]	NVARCHAR(60)    NOT NULL,
    CONSTRAINT [PK_dbo.MotivosDeCorte] PRIMARY KEY CLUSTERED ([Id] ASC)
);