CREATE TABLE [dbo].[TaraContenedor] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL, 
    [CodigoContenedor] NVARCHAR(10) NOT NULL, 
    [Descripcion] NVARCHAR(20) NOT NULL, 
    [PesoTara] INT NOT NULL,
	CONSTRAINT [PK_TaraContenedor] PRIMARY KEY CLUSTERED ([Id] ASC),
);

