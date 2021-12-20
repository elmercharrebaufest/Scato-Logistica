CREATE TABLE [dbo].[TaraRomaneo](
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[Codigo]			INT NOT NULL,
	[Descripcion]		NVARCHAR(30) NOT NULL,
	[Peso]				DECIMAL(18, 0) NOT NULL,
	[Importacion]		BIT NULL,
	[CargaPesoManual]	BIT NULL,
	[Centro_Id]			INT NULL,
 CONSTRAINT [PK_TaraRomaneo] PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [FK_dbo.TaraRomaneo_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
 );


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[TaraRomaneo]([Centro_Id] ASC);
