CREATE TABLE [dbo].[ConversionCentro]
(
	[Id]			INT IDENTITY (1, 1) NOT NULL,
    [Camara_Id]		INT NOT NULL,
	[Centro_Id]	INT NOT NULL,
	[CodigoCamara]	NVARCHAR(10) NOT NULL,
	CONSTRAINT [PK_dbo.ConversionCentro] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT	[FK_dbo.ConversionCentro_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
	CONSTRAINT	[FK_dbo.ConversionCentro_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
)
