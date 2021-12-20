CREATE TABLE [dbo].[AnalisisDeCalidad] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [NumeroOrden]      NVARCHAR (40) NOT NULL,
	[WorkflowInstanceId]    UNIQUEIDENTIFIER NOT NULL,
	[Calado_Id] INT            NOT NULL,
	[FechaCreacion] DATETIME   NOT NULL DEFAULT GETDATE(),
	[Usuario]	         NVARCHAR (30) NULL, 
    CONSTRAINT [PK_dbo.AnalisisDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.AnalisisDeCalidad_dbo.Calado_Calado_Id] FOREIGN KEY ([Calado_Id]) REFERENCES [dbo].[Calado] ([Id]) ON DELETE CASCADE
);