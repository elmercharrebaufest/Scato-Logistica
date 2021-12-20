CREATE TABLE [dbo].[CaracteristicaDeCalidadPorWorkflow] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CaracteristicaDeCalidad_Id] INT NOT NULL,
    [Workflow_Id] INT NOT NULL 
    CONSTRAINT [PK_dbo.CaracteristicaDeCalidadPorWorkflow] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CaracteristicaDeCalidadPorWorkflow_dbo.CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]),
	CONSTRAINT [FK_dbo.CaracteristicaDeCalidadPorWorkflow_dbo.Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);

