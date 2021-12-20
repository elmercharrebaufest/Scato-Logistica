CREATE TABLE [dbo].[GeneradorNumeroOrdenDeDescarga] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroOrdenDeDescarga] PRIMARY KEY CLUSTERED ([Id] ASC)
);

