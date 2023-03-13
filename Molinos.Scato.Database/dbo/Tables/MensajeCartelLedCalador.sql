CREATE TABLE [dbo].[MensajeCartelLedCalador]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[MensajeCartelLed_Id] INT NOT NULL,
    [Calle_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.MensajeCartelLedCalador_dbo.MensajeCartelLed_Id] FOREIGN KEY ([MensajeCartelLed_Id]) REFERENCES [dbo].[MensajeCartelLed] ([Id]),
	CONSTRAINT [FK_dbo.MensajeCartelLedCalador_dbo.Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id]),
)
