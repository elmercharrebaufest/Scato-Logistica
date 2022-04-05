CREATE TABLE [dbo].[LogActividad] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId] UNIQUEIDENTIFIER NOT NULL,
    [Actividad]          NVARCHAR (100)   NOT NULL,
    [Fecha]              DATETIME         NOT NULL,
    [ActividadXaml]      NVARCHAR (100)   NULL,
    CONSTRAINT [PK_dbo.LogActividad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LogActividad_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [IX_LogActividad_WorkflowInstanceId]
    ON [dbo].[LogActividad]([WorkflowInstanceId] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);



GO

CREATE NONCLUSTERED INDEX [IX_LogActividad_ActividadXaml]
    ON [dbo].[LogActividad]([ActividadXaml] ASC)
    INCLUDE([WorkflowInstanceId], [Actividad]) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
