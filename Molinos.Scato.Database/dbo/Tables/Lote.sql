CREATE TABLE [dbo].[Lote] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [NumeroDeLote] NVARCHAR (10) NOT NULL,
    [Camara_Id]    INT           NOT NULL,
    [Fecha]        DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.Lote] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Lotea_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id])
);

