CREATE TABLE [dbo].[WorkflowTipoComercial] (
    [Workflow_Id]      INT NOT NULL,
    [TipoComercial_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.WorkflowTipoComercial] PRIMARY KEY CLUSTERED ([Workflow_Id] ASC, [TipoComercial_Id] ASC),
    CONSTRAINT [FK_dbo.WorkflowTipoComercial_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.WorkflowTipoComercial_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Workflow_Id]
    ON [dbo].[WorkflowTipoComercial]([Workflow_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TipoComercial_Id]
    ON [dbo].[WorkflowTipoComercial]([TipoComercial_Id] ASC);

