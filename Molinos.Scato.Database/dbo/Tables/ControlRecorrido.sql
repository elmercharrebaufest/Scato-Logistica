CREATE TABLE [dbo].[ControlRecorrido] (
    [Id]                     INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]     UNIQUEIDENTIFIER NOT NULL,
    [Mensaje]				 NVARCHAR (100)   NULL,
    [Comentario]			 NVARCHAR (MAX)   NULL,
    [NombreUsuario]          NVARCHAR (20)    NULL,
    [Actividad]				 NVARCHAR (50)    NULL,
    [ActividadXaml]		     NVARCHAR (50)    NULL,
    [Decision]               BIT              NOT NULL,
	[Fecha]					DATETIME NULL, 
	[PuestoDeTrabajo_Id]	 INT  NULL,
    CONSTRAINT [PK_dbo.ControlRecorrido] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ControlRecorrido_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]),
	CONSTRAINT [FK_dbo.ControlRecorrido_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);

GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[ControlRecorrido]([PuestoDeTrabajo_Id] ASC);
GO
CREATE NONCLUSTERED INDEX IX_Mensaje_Actividad
ON [dbo].[ControlRecorrido] ([Mensaje],[Actividad])
INCLUDE ([WorkflowInstanceId])
GO
CREATE INDEX [IX_ControlRecorrido_WorkflowInstanceId] ON [dbo].[ControlRecorrido] ([WorkflowInstanceId])
GO
CREATE INDEX [IX_ControlRecorrido_WorkflowInstanceId_ActividadXaml] ON [dbo].[ControlRecorrido] ([WorkflowInstanceId],ActividadXaml, puestodetrabajo_id)
GO
