CREATE TABLE [dbo].[CeldaManoDeEmbarque] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,
    [Posicion]     INT NULL,
    CONSTRAINT [PK_dbo.CeldaManoDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_CeldaManoDeEmbarque_Nombre] UNIQUE (Nombre)
);
GO