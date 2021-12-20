CREATE TABLE [dbo].[Calado] (
    [Id]                   INT  IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]   UNIQUEIDENTIFIER		NOT NULL,
	[CicloDeCalado]        INT					NOT NULL,
	[NumeroOrden]		   NVARCHAR (40)		NOT NULL,
	[MuestraConjunto]      INT					NULL,
    [EstaAutorizadoPorEntregador] BIT			NULL, 
	[FechaCreacion]		   DATETIME				NULL,
	[CalidadMaterial_Id]   INT					NULL,
	[Usuario]			   NVARCHAR (30)		NULL, 
	[Comentario]		   NVARCHAR (100)		NULL, 

    CONSTRAINT [PK_dbo.Calado] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Calado_dbo.CalidadMaterial_CalidadMaterial_Id] FOREIGN KEY ([CalidadMaterial_Id]) REFERENCES [dbo].[CalidadMaterial] ([Id]),
	CONSTRAINT [FK_dbo.Calado_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_Calado_WorkflowInstanceId] ON [dbo].[Calado] ([WorkflowInstanceId])
