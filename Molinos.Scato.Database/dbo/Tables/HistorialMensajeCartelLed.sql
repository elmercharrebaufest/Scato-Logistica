CREATE TABLE [dbo].[HistorialMensajeCartelLed]
(
	[Id] INT  NOT NULL, 
    [Mensaje] NVARCHAR(50) NULL, 
    [Calle_Id] INT NULL, 
    [FechaUltimaModificacion] DATETIME NULL, 
    [Recorrido_Id] INT NULL, 
    CONSTRAINT [PK_dbo.HistorialMensajeCartelLed] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.HistorialMensajeCartelLede_dbo.HistorialMensajeCartelLed_MensajeCartelLed_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[MensajeCartelLed] ([Id]),
    CONSTRAINT [FK_dbo.HistorialMensajeCartelLede_dbo.HistorialMensajeCartelLed_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id]),
    CONSTRAINT [FK_dbo.HistorialMensajeCartelLede_dbo.HistorialMensajeCartelLed_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
)
