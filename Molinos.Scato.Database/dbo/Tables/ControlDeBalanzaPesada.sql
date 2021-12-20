CREATE TABLE [dbo].[ControlDeBalanzaPesada] (
    [Id] INT   IDENTITY (1, 1) NOT NULL,
	[Balanza_Id]		INT NOT NULL,
	CONSTRAINT [FK_dbo.Recorrido_dbo.ControlDeBalanza_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id]),
	[Peso]		 INT NOT NULL,
	[Fecha] DATETIME NOT NULL,
	[ControlDeBalanza_Id] INT NOT NULL,
	CONSTRAINT [FK_dbo.ControlDeBalanzaPesada_ControlDeBalanza_Id] FOREIGN KEY ([ControlDeBalanza_Id]) REFERENCES [dbo].[ControlDeBalanza] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_dbo.ControlDeBalanzaPesada] PRIMARY KEY CLUSTERED ([Id] ASC)
);