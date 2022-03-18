CREATE TABLE [dbo].[GeneradorNumeroOrdenDeDescargaFason] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [SeqVal] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroOrdenDeDescargaFason] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



