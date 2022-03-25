CREATE TABLE [dbo].[LogSincronizacion] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [NombreInterface] NVARCHAR (50)  NOT NULL,
    [FechaEjecucion]  DATETIME       NOT NULL,
    [Completa]        BIT            DEFAULT ((0)) NOT NULL,
    [Correcta]        BIT            DEFAULT ((0)) NOT NULL,
    [Mensaje]         NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


