CREATE TABLE [dbo].[Observacion] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]    UNIQUEIDENTIFIER NOT NULL,
    [Observaciones]        NVARCHAR (1000) NULL,
    CONSTRAINT [PK_dbo.Observacion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_Observacion_WorkflowInstanceId] UNIQUE ([WorkflowInstanceId]),
	CONSTRAINT [FK_dbo.Observacion_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_Observacion_WorkflowInstanceId] ON [dbo].[Observacion] ([WorkflowInstanceId])



