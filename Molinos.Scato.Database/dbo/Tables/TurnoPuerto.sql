CREATE TABLE [dbo].[TurnoPuerto] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,
    [Orden] int not NULL,
    CONSTRAINT [PK_dbo.TurnoPuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_TurnoPuerto_Nombre] UNIQUE (Nombre)
);
GO