CREATE TABLE [dbo].[TipoDeBuquePuerto] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NULL,    
    CONSTRAINT [PK_dbo.TipoDeBuquePuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_TipoDeBuquePuerto_Nombre] UNIQUE (Nombre)
);
GO