CREATE TABLE [dbo].[Lector] (
    [Id]                        INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]               NVARCHAR (35) NOT NULL,
    CONSTRAINT [PK_dbo.Lector] PRIMARY KEY CLUSTERED ([Id] ASC)
);