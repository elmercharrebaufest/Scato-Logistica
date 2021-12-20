CREATE TABLE [dbo].[Bodega] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NULL,
    
    CONSTRAINT [PK_dbo.Bodega] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_Bodega_Nombre] UNIQUE (Nombre) 
);
GO