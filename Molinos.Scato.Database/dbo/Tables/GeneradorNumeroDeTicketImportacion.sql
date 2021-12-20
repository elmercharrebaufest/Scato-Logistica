CREATE TABLE [dbo].[GeneradorNumeroDeTicketImportacion]
(
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroDeTicketImportacion] PRIMARY KEY CLUSTERED ([Id] ASC)
)
