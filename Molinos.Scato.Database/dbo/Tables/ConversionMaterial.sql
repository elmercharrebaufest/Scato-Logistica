CREATE TABLE [dbo].[ConversionMaterial] (
    [Id]			INT IDENTITY (1, 1) NOT NULL,
    [Camara_Id]		INT NOT NULL,
	[Material_Id]	INT NOT NULL,
	[CodigoCamara]	NVARCHAR(10) NOT NULL,
	CONSTRAINT [PK_dbo.ConversionMaterial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT	[FK_dbo.ConversionMaterial_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
	CONSTRAINT	[FK_dbo.ConversionMaterial_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
);