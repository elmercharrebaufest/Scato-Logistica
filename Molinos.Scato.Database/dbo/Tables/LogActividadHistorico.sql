CREATE TABLE [dbo].[LogActividadHistorico] (
    [Id]                 INT              NOT NULL,
    [WorkflowInstanceId] UNIQUEIDENTIFIER NOT NULL,
    [Actividad]          NVARCHAR (100)   NOT NULL,
    [Fecha]              DATETIME         NOT NULL,
    [ActividadXaml]      NVARCHAR (100)   NULL,
    CONSTRAINT [PK_dbo.LogActividadHistorico] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_dbo.LogActividadHistorico_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [IX_LogActividadHistorico_WorkflowInstanceId]
    ON [dbo].[LogActividadHistorico]([WorkflowInstanceId] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO

CREATE INDEX [IX_LogActividadHistorico_ActividadXaml]
ON [dbo].[LogActividadHistorico] ([ActividadXaml])
INCLUDE ([WorkflowInstanceId],[Actividad])
GO

GO
CREATE NONCLUSTERED INDEX [IX_LogActividadHistorico_ActividadXam_Fecha]
ON [dbo].[LogActividadHistorico] ([ActividadXaml],[Fecha])
INCLUDE ([WorkflowInstanceId])
GO
