CREATE TABLE [dbo].[ColaIdentificacionVehicular] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [PuestoDeTrabajo_Id]    INT             NOT NULL,
    [Patente]               NVARCHAR (15)   NULL,
    [ReconocimientoExitoso] BIT             DEFAULT ((0)) NOT NULL,
    [MensajeError]          NVARCHAR (500)  NULL,
    [Imagen]                VARBINARY (MAX) NULL,
    [FechaEncolado]         DATETIME        DEFAULT (GETDATE()) NOT NULL,
    CONSTRAINT [PK_ColaIdentificacionVehicular] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ColaIdentificacionVehicular_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_ColaIdentificacionVehicular_PuestoDeTrabajo_Id]
    ON [dbo].[ColaIdentificacionVehicular]([PuestoDeTrabajo_Id] ASC, [FechaEncolado] ASC)
    WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);
