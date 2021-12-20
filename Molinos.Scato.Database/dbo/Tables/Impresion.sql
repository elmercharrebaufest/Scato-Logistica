CREATE TABLE [dbo].[Impresion] (
    [Id]                     INT             IDENTITY (1, 1) NOT NULL,
	[Impresora]				NVARCHAR (50)    NULL,
	[TipoImpresion]			INT    NULL,
	[FechaImpresion]		DATETIME NOT NULL,
	[Patente]				NVARCHAR (50)    NULL,
	[Codigo]				NVARCHAR (40)   NOT NULL default '',
    [WorkflowId]			UNIQUEIDENTIFIER NULL,
	[Eliminada]				BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_dbo.Impresion] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Impresion_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow])

);

GO

CREATE INDEX [IX_Impresion_WorkflowInstanceId] ON [dbo].[Impresion] ([WorkflowId])