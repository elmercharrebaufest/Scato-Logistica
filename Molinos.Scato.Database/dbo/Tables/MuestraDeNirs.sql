CREATE TABLE [dbo].[MuestraDeNirs] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]         UNIQUEIDENTIFIER NOT NULL,
	[Centro_Id]          INT    NULL,
	[Nirs_Id]          INT    NULL,
	[Modalidad]          NVARCHAR (30)    NULL,
	[NumeroOrden]          NVARCHAR (30)    NULL,
	[NumeroDocumentoIngreso]          NVARCHAR (30)    NULL,
	[CicloDeCalado]              INT              NULL,
	[NroDeToma]              INT              NULL,
	[ValorLeido]                      DECIMAL (18, 2)  NULL,
	[ValorFinal]                      DECIMAL (18, 2)  NULL,
	[Fecha]                      DATETIME        NULL,
	[Usuario]          NVARCHAR (30)    NULL
    CONSTRAINT [PK_dbo.MuestraDeNirs] PRIMARY KEY CLUSTERED ([Id] ASC),	
    CONSTRAINT [FK_dbo.MuestraDeNirs_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.MuestraDeNirs_dbo.Nirs_Nirs_Id] FOREIGN KEY ([Nirs_Id]) REFERENCES [dbo].[Nirs] ([Id]),
	CONSTRAINT [FK_dbo.MuestraDeNirs_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_MuestraDeNirs_WorkflowInstanceId] ON [dbo].[MuestraDeNirs] ([WorkflowInstanceId])
