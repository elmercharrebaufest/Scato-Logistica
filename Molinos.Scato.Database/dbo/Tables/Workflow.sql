CREATE TABLE [dbo].[Workflow] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [Codigo]           NVARCHAR (100) NOT NULL,
    [Descripcion]      NVARCHAR (100) NOT NULL,
	[TipoDeWorkflow]   INT			 NOT NULL,
	[Centro_Id] INT NOT NULL default 1,
    [Activo] BIT NOT NULL CONSTRAINT DF_Workflow_Activo DEFAULT(1), 
    [PendienteNoGranos] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.Workflow] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Workflow_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [UK_dbo.Workflow] UNIQUE ([Codigo])
);