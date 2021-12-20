CREATE TABLE [dbo].[Pais] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_dbo.Pais] PRIMARY KEY CLUSTERED ([Id] ASC)
);

