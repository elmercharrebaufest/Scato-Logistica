CREATE TABLE [dbo].[GeneradorNumeroDocumentoFason] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [SeqVal] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroDocumentoFason] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



