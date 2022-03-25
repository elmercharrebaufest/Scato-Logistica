CREATE TABLE [dbo].[MaterialPorWorkflow] (
    [Id]                             INT IDENTITY (1, 1) NOT NULL,
    [Centro_Id]                      INT NOT NULL,
    [Material_Id]                    INT NOT NULL,
    [Workflow_Id]                    INT NOT NULL,
    [Cliente_Id]                     INT NULL,
    [EnviaASapAlmacenPredeterminado] BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.MaterialPorWorkflow] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MaterialPorWorkflow_dbo.Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.MaterialPorWorkflow_dbo.Cliente_Id] FOREIGN KEY ([Cliente_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_dbo.MaterialPorWorkflow_dbo.Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.MaterialPorWorkflow_dbo.Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);



