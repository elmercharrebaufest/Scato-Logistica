CREATE TABLE [dbo].[WorkflowDefinicion] (
    [Id]					INT IDENTITY (1, 1) NOT NULL,
	[Workflow_Id]			INT NOT NULL,
	[FechaCreacion]			DATETIME NOT NULL,
	[Comentario]			NVARCHAR (1000) NULL,
    [NombreUsuario]			NVARCHAR (20) NOT NULL,
	[Activa]				BIT NOT NULL CONSTRAINT DF_WorkflowDefinicion_Activa DEFAULT(0),
	[FechaActivacion] DATETIME NOT NULL  DEFAULT(GETDATE()), 
	[ActividadInicial]		NVARCHAR (100) NOT NULL,
	[Definicion]			VARBINARY(MAX) NOT NULL,
    CONSTRAINT [PK_dbo.WorkflowDefinicion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.WorkflowDefinicion_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);