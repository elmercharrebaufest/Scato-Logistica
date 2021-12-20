CREATE TABLE [dbo].[MovimientoDeTerceros] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id]         INT NOT NULL,
	[ArchivoDeMovimientos_Id]         INT NOT NULL,
	CONSTRAINT [PK_dbo.MovimientoDeTerceros] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MovimientoDeTerceros_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.MovimientoDeTerceros_dbo.ArchivoDeMovimientos_ArchivoDeMovimientos_Id] FOREIGN KEY ([ArchivoDeMovimientos_Id]) REFERENCES [dbo].[ArchivoDeMovimientos] ([Id]),
	
);