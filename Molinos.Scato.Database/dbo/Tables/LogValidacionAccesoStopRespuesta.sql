CREATE TABLE [dbo].[LogValidacionAccesoStopRespuesta] (
    [Id]                                      INT            IDENTITY (1, 1) NOT NULL,
    [LogValidacionAccesoStopBandasHorariasId] INT            NOT NULL,
    [RespuestaStop]                           NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.LogValidacionAccesoStopRespuesta] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LogValidacionAccesoStopRespuesta_dbo.LogValidacionAccesoStopBandasHorarias_Id] FOREIGN KEY ([LogValidacionAccesoStopBandasHorariasId]) REFERENCES [dbo].[LogValidacionAccesoStopBandasHorarias] ([Id])
);
