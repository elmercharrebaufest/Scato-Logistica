CREATE TABLE [dbo].[SentidoManoDeEmbarque] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL, 
    [Posicion]     INT NULL,
    CONSTRAINT [PK_dbo.SentidoManoDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_SentidoManoDeEmbarque_Nombre] UNIQUE (Nombre)
);
GO
