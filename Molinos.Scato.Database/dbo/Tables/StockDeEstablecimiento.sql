CREATE TABLE [dbo].[StockDeEstablecimiento] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [CodigoEstablecimiento] NVARCHAR (10)   NOT NULL,
    [Cosecha]               NVARCHAR (10)   NOT NULL,
    [FechaDesde]            DATETIME        NOT NULL,
    [FechaHasta]            DATETIME        NOT NULL,
    [StockDeclarado]        DECIMAL (18, 2) NOT NULL,
    [StockReservado]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Tolerancia] DECIMAL(18, 2) NULL, 
    CONSTRAINT [PK_dbo.StockDeEstablecimiento] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

