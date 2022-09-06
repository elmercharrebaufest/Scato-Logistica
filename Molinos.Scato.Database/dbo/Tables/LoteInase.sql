CREATE TABLE [dbo].[LoteInase] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [NumeroDeLote]  NVARCHAR (20) NOT NULL,
    [NombreUsuario] NVARCHAR (20) NOT NULL,
    [Fecha]    DATETIME      NOT NULL,
    [Centro_Id]     INT           NOT NULL,
    CONSTRAINT [PK_dbo.LoteInase] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LoteInase_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
);

