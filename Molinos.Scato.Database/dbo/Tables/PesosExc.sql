CREATE TABLE [dbo].[PesosExc]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Recorrido_Id] INT NOT NULL, 
    [PesoTomado] INT NOT NULL,
    CONSTRAINT [PK_dbo.PesosExc] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PesosExc_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
)
