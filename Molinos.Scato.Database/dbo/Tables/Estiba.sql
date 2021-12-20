CREATE TABLE [dbo].[Estiba] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]					 NVARCHAR(50)             NOT NULL,
    [Apellido]                       NVARCHAR(50)            NOT NULL,
    CONSTRAINT [PK_dbo.Estiba] PRIMARY KEY CLUSTERED ([Id] ASC)
);
