CREATE TABLE [dbo].[GeneradorNumeroOrdenDeCargaContenedor] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroOrdenDeCargaContenedor] PRIMARY KEY CLUSTERED ([Id] ASC)
);

