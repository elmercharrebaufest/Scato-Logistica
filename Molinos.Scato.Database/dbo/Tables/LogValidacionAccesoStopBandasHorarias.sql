CREATE TABLE [dbo].[LogValidacionAccesoStopBandasHorarias] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [CTG]                   NVARCHAR (50)  NOT NULL,
    [Patente]               NVARCHAR (20)  NOT NULL,
    [FechaIngreso]          DATETIME       NOT NULL,
    [Permitido]             BIT            NULL,
    [Semaforo]              NVARCHAR (20)  NULL,
    [Estado]                NVARCHAR (100) NULL,
    [Mensaje]               NVARCHAR (MAX) NULL,
    [BandaHorariaFecha]     DATETIME       NULL,
    [BandaHorariaHoraDesde] NVARCHAR (20)  NULL,
    [BandaHorariaHoraHasta] NVARCHAR (20)  NULL,
    CONSTRAINT [PK_dbo.LogValidacionAccesoStopBandasHorarias] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
);
