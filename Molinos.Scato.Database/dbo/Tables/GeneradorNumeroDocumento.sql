CREATE TABLE [dbo].[GeneradorNumeroDocumento] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroDocumento] PRIMARY KEY CLUSTERED ([Id] ASC)
);

