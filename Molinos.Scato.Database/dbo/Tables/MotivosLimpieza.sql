CREATE TABLE [dbo].[MotivosLimpieza] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,    
    CONSTRAINT [PK_dbo.MotivosLimpieza] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_MotivosLimpieza_Nombre] UNIQUE (Nombre)
);
GO