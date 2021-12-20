CREATE TABLE [dbo].[ActividadConCargaAutomatica] (
    [Id]                   INT IDENTITY (1, 1) NOT NULL,
	[Workflow_Id]	       INT  NOT NULL,
    [Actividad]            NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.ActividadConCargaAutomatica] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ActividadConCargaAutomatica_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Workflow_Id]
    ON [dbo].[ActividadConCargaAutomatica]([Workflow_Id] ASC);
