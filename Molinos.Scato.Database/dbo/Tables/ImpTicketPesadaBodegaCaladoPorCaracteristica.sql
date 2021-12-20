CREATE TABLE [dbo].[ImpTicketPesadaBodegaCaladoPorCaracteristica] (
    [ImpTicketPesadaBodega_Id]      INT NOT NULL,
    [CaladoPorCaracteristica_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpTicketPesadaBodegaCaladoPorCaracteristica] PRIMARY KEY CLUSTERED ([ImpTicketPesadaBodega_Id] ASC, [CaladoPorCaracteristica_Id] ASC),
    CONSTRAINT [FK_dbo.ImpTicketPesadaBodegaCaladoPorCaracteristica_dbo.CaladoPorCaracteristica_CaladoPorCaracteristica_Id] FOREIGN KEY ([CaladoPorCaracteristica_Id]) REFERENCES [dbo].[CaladoPorCaracteristica] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ImpTicketPesadaBodegaCaladoPorCaracteristica_dbo.ImpTicketPesadaBodega_ImpTicketPesadaBodega_Id] FOREIGN KEY ([ImpTicketPesadaBodega_Id]) REFERENCES [dbo].[ImpTicketPesadaBodega] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ImpTicketPesadaBodega_Id]
    ON [dbo].[ImpTicketPesadaBodegaCaladoPorCaracteristica]([ImpTicketPesadaBodega_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CaladoPorCaracteristica_Id]
    ON [dbo].[ImpTicketPesadaBodegaCaladoPorCaracteristica]([CaladoPorCaracteristica_Id] ASC);
