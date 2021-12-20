CREATE TABLE [dbo].[DescargaUnidad] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
	[WorkflowInstanceId]   UNIQUEIDENTIFIER NOT NULL,
    [NroPedido]		    NVARCHAR (10)  NULL,
    [Observaciones]		NVARCHAR (1000) NULL,
	[Estado]			INT NOT NULL DEFAULT 0,
	[FechaInicio]		DATETIME NOT NULL DEFAULT GETDATE(),
	[FechaCierre]		DATETIME NULL,
	CONSTRAINT [PK_dbo.DescargaUnidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	[Proveedor_Id] INT NOT NULL, 
	CONSTRAINT [FK_dbo.DescargaUnidad_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
	CONSTRAINT [FK_dbo.DescargaUnidad_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_DescargaUnidad_WorkflowInstanceId] ON [dbo].[DescargaUnidad] ([WorkflowInstanceId])