CREATE TABLE [dbo].[CoordinadorPuerto] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,    
    CONSTRAINT [PK_dbo.CoordinadorPuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_CoordinadorPuerto_Nombre] UNIQUE (Nombre)
);
GO