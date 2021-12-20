CREATE TABLE [dbo].[Precinto] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]    UNIQUEIDENTIFIER NOT NULL,
    [NumeroPrecinto]       NVARCHAR (12) NOT NULL,
    [Detalle]              NVARCHAR (20) NULL,
    CONSTRAINT [PK_dbo.Precinto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Precinto_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE

);
GO

CREATE INDEX [IX_Precinto_WorkflowInstanceId] ON [dbo].[Precinto] ([WorkflowInstanceId])




