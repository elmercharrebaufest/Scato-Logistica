CREATE TABLE [dbo].[Impresora]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Descripcion] NVARCHAR(80) NOT NULL,
	[Direccion] NVARCHAR(80) NOT NULL,
    [Centro_Id] INT NOT NULL,
	[IsZebra] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_dbo.Impresora_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
)
