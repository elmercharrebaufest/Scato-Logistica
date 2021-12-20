CREATE TABLE [dbo].[ControlDeBalanza] (
    [Id]		     INT IDENTITY (1, 1) NOT NULL,
	[Observaciones]	 NVARCHAR(250),
	[TipoPesada] INT NOT NULL,
    [Recorrido_Id]   INT NOT NULL,
    CONSTRAINT [FK_dbo.ControlDeBalanza_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_dbo.ControlDeBalanza] PRIMARY KEY CLUSTERED ([Id] ASC)
);