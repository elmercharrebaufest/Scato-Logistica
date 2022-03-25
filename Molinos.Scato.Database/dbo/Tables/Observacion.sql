CREATE TABLE [dbo].[Observacion] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId] UNIQUEIDENTIFIER NOT NULL,
    [Observaciones]      NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.Observacion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Observacion_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE,
    CONSTRAINT [UK_Observacion_WorkflowInstanceId] UNIQUE NONCLUSTERED ([WorkflowInstanceId] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON)
);


GO

CREATE NONCLUSTERED INDEX [IX_Observacion_WorkflowInstanceId]
    ON [dbo].[Observacion]([WorkflowInstanceId] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);





