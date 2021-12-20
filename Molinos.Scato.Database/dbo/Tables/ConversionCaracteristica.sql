CREATE TABLE [dbo].[ConversionCaracteristica] (
    [Id]			INT IDENTITY (1, 1) NOT NULL,
    [Camara_Id]		INT NOT NULL,
	[Material_Id]	INT NOT NULL,
	[CodigoCamara]	NVARCHAR(10) NOT NULL,
	[Caracteristica_Id] INT NOT NULL,
	CONSTRAINT [PK_dbo.ConversionCaracteristica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT	[FK_dbo.ConversionCaracteristica_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
	CONSTRAINT	[FK_dbo.ConversionCaracteristica_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT	[FK_dbo.ConversionCaracteristica_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([Caracteristica_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]),
);