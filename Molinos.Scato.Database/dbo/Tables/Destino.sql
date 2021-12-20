CREATE TABLE [dbo].[Destino] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NULL,
    
    CONSTRAINT [PK_dbo.Destino] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_Destino_Nombre] UNIQUE (Nombre) 
);
GO