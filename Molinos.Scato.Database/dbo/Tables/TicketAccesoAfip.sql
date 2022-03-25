CREATE TABLE [dbo].[TicketAccesoAfip] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Token]            NVARCHAR (MAX) NOT NULL,
    [Sign]             NVARCHAR (MAX) NOT NULL,
    [Service]          NVARCHAR (MAX) NULL,
    [CuitRepresentado] NVARCHAR (MAX) NOT NULL,
    [ExpirationTime]   DATETIME       NOT NULL,
    [GenerationTime]   DATETIME       NOT NULL,
    [FechaCreacion]    DATETIME       NULL,
    CONSTRAINT [PK_dbo.TicketAccesoAfip] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



