CREATE TABLE [dbo].[CaladoEnPlanta] (
    [Id]                   INT  IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]   UNIQUEIDENTIFIER		NOT NULL,
	[FechaCreacion]		   DATETIME				NULL,
	[Usuario]			   NVARCHAR (30)		NULL, 
	[Comentario]		   NVARCHAR (100)		NULL, 

    CONSTRAINT [PK_dbo.CaladoEnPlanta] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CaladoEnPlanta_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_CaladoEnPlanta_WorkflowInstanceId] ON [dbo].[CaladoEnPlanta] ([WorkflowInstanceId])
