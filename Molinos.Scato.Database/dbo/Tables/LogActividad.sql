CREATE TABLE [dbo].[LogActividad] (
    [Id]                     INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]     UNIQUEIDENTIFIER NOT NULL,
    [Actividad]				 NVARCHAR (100)    NOT NULL,
	[Fecha]					DATETIME NOT NULL, 
    [ActividadXaml]			NVARCHAR(100) NULL, 
    CONSTRAINT [PK_dbo.LogActividad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.LogActividad_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_LogActividad_WorkflowInstanceId] ON [dbo].[LogActividad] ([WorkflowInstanceId])

GO

CREATE INDEX [IX_LogActividad_ActividadXaml]
ON [dbo].[LogActividad] ([ActividadXaml])
INCLUDE ([WorkflowInstanceId],[Actividad])
GO
