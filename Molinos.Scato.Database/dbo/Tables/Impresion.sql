CREATE TABLE [dbo].[Impresion] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [Impresora]      NVARCHAR (50)    NULL,
    [TipoImpresion]  INT              NULL,
    [FechaImpresion] DATETIME         NOT NULL,
    [Patente]        NVARCHAR (50)    NULL,
    [Codigo]         NVARCHAR (40)    DEFAULT ('') NOT NULL,
    [WorkflowId]     UNIQUEIDENTIFIER NULL,
    [Eliminada]      BIT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Impresion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Impresion_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow])
);



GO

CREATE INDEX [IX_Impresion_WorkflowInstanceId] ON [dbo].[Impresion] ([WorkflowId])
GO
CREATE NONCLUSTERED INDEX [ndx_WorkflowId_TipoImpresion_Eliminada_Id_Impresora]
    ON [dbo].[Impresion]([WorkflowId] ASC, [TipoImpresion] ASC, [Eliminada] ASC)
    INCLUDE([Id], [Impresora], [FechaImpresion], [Patente], [Codigo]) WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [ndx_TipoImpresion_Workflowid_Eliminada_Id_Impresora]
    ON [dbo].[Impresion]([TipoImpresion] ASC, [WorkflowId] ASC, [Eliminada] ASC)
    INCLUDE([Id], [Impresora], [FechaImpresion], [Patente], [Codigo]) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [ndx_Eliminada_TipoImpresion_WorkflowId]
    ON [dbo].[Impresion]([Eliminada] ASC, [TipoImpresion] ASC, [WorkflowId] ASC)
    INCLUDE([Id], [Impresora], [FechaImpresion], [Patente], [Codigo]) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

