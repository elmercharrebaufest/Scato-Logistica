CREATE TABLE [dbo].[ControlDeTiempo] (
    [Id] INT   IDENTITY (1, 1) NOT NULL,
	[ActividadDesde]		NVARCHAR (40) NOT NULL,
	[ActividadHasta]		NVARCHAR (40) NOT NULL,
	[CodigoControl]		NVARCHAR (40) NOT NULL,
    [TiempoMaximo]      INT NOT NULL,
    [Workflow_Id]      INT NOT NULL,
	CONSTRAINT [PK_dbo.ControlDeTiempo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ControlDeTiempo_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Workflow_Id]
    ON [dbo].[ControlDeTiempo]([Workflow_Id] ASC);

