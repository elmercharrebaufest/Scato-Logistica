CREATE TABLE [dbo].[SentidoManoDeEmbarque] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]   NVARCHAR (60) NOT NULL,
    [Posicion] INT           NULL,
    CONSTRAINT [PK_dbo.SentidoManoDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [UK_SentidoManoDeEmbarque_Nombre] UNIQUE NONCLUSTERED ([Nombre] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


GO
