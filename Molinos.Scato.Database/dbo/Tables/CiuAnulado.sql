CREATE TABLE [dbo].[CiuAnulado] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Numero]                    NVARCHAR (8) NOT NULL,
    [Fecha]						DATETIME NOT NULL, 
    CONSTRAINT [PK_dbo.CiuAnulado] PRIMARY KEY CLUSTERED ([Id] ASC),
);