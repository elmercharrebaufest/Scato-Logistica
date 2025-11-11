CREATE TABLE [dbo].[VisecTransmision] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [CUITEmpresa]               NVARCHAR (50)  NULL,
    [NumeroProceso]             NVARCHAR (50)  NULL,
    [Estado]                    INT            NOT NULL,
    [FechaTransaccion]          DATETIME       NULL,
    [DetalleTransaccion]        NVARCHAR (MAX) NULL,
    [HistorialProcesos]         NVARCHAR (MAX) NULL,
    [FechaHoraMovimiento]       DATETIME       NOT NULL,
    [FechaCPE]                  DATETIME       NOT NULL,
    [NumeroCPE]                 NVARCHAR (14)  NULL,
    [NumeroCTG]                 NVARCHAR (12)  NULL,
    [CUITTitular]               NVARCHAR (50)  NULL,
    [NumeroRUCAOrigen]          INT            NULL,
    [CUITDestinatario]          NVARCHAR (50)  NULL,
    [CUITDestino]               NVARCHAR (50)  NULL,
    [NumeroRUCADestino]         INT            NOT NULL,
    [Producto]                  INT            NOT NULL,
    [Campania]                  NVARCHAR (5)   NULL,
    [PesoNetoCargaKg]           INT            NOT NULL,
    [StockKg] INT NULL, 
    CONSTRAINT [PK_dbo.VisecTransmision] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

GO
CREATE NONCLUSTERED INDEX [IX_Estado]
    ON [dbo].[VisecTransmision]([Estado] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FechaTransaccion]
    ON [dbo].[VisecTransmision]([FechaTransaccion] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_NumeroProceso]
    ON [dbo].[VisecTransmision]([NumeroProceso] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_NumeroCTG]
    ON [dbo].[VisecTransmision]([NumeroCTG] ASC);
