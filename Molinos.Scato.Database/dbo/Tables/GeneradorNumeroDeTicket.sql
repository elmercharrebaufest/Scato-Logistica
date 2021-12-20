CREATE TABLE [dbo].[GeneradorNumeroDeTicket]
(
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroDeTicket] PRIMARY KEY CLUSTERED ([Id] ASC)
)
