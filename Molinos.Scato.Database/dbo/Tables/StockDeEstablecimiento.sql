CREATE TABLE [dbo].[StockDeEstablecimiento] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[CodigoEstablecimiento]         NVARCHAR(10) NOT NULL,
    [Cosecha]         NVARCHAR (10) NOT NULL,
	[FechaDesde]  DATETIME NOT NULL,
    [FechaHasta]  DATETIME NOT NULL,
    [StockDeclarado]    DECIMAL(18, 2) NOT NULL,
	[StockReservado] DECIMAL(18, 2) NOT NULL default 0,
    CONSTRAINT [PK_dbo.StockDeEstablecimiento] PRIMARY KEY CLUSTERED ([Id] ASC)
);