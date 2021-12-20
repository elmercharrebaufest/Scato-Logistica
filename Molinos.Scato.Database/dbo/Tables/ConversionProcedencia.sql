CREATE TABLE [dbo].[ConversionProcedencia] (
    [Id]			INT IDENTITY (1, 1) NOT NULL,
    [Camara_Id]		INT NOT NULL,
	[Procedencia_Id]	INT NOT NULL,
	[CodigoCamara]	NVARCHAR(10) NOT NULL,
	CONSTRAINT [PK_dbo.ConversionProcedencia] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT	[FK_dbo.ConversionProcedencia_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
	CONSTRAINT	[FK_dbo.ConversionProcedencia_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Procedencia_Id]) REFERENCES [dbo].[Localidad] ([Id]),
);