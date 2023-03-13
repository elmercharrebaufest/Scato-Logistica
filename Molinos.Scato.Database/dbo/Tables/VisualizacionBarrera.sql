CREATE TABLE [dbo].[VisualizacionBarrera]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Codigo] NCHAR(100) NOT NULL,
	[Descripcion] NCHAR(150) NOT NULL,
	[Rol_Id] INT NOT NULL,
	[Deshabilitada] BIT NOT NULL default 0,
	[Visible] BIT NOT NULL default 0,
	[CentroId] INT NOT NULL,

	CONSTRAINT [PK_dbo.VisualizacionBarrera] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.VisualizacionBarrera_dbo.Rol_Rol_Id] FOREIGN KEY ([Rol_Id]) REFERENCES [dbo].[Rol] ([Id]),
	CONSTRAINT [FK_dbo.VisualizacionBarrera_dbo.Centro_Centro_Id] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id])
);
