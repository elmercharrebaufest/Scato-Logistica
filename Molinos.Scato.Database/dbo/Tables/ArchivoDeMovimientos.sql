CREATE TABLE [dbo].[ArchivoDeMovimientos] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [NumeroDeArchivo]		NVARCHAR (20) NOT NULL,
	[NombreUsuario]		NVARCHAR (20) NOT NULL,
	[TipoDeWorkflow]         INT NOT NULL,
	[Material_Id]         INT NOT NULL,
    [Fecha]				DATETIME NOT NULL,
	[FechaDesde]				DATETIME NOT NULL,
	[FechaHasta]				DATETIME NOT NULL,
	[Centro_Id]			INT NOT NULL, 
    CONSTRAINT [PK_dbo.ArchivoDeMovimientos] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT	[FK_dbo.ArchivoDeMovimientos_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT	[FK_dbo.ArchivoDeMovimientos_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);