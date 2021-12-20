CREATE TABLE [dbo].[Empresa] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (30)   NOT NULL,
   
    CONSTRAINT [PK_dbo.Empresa] PRIMARY KEY CLUSTERED ([Id] ASC),
);