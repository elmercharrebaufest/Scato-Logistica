CREATE TABLE [dbo].[LogIngresoPorPuesto]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Recorrido_Id] INT NOT NULL,
    [PuestoDeTrabajo_Id] INT NOT NULL,
    [TipoIngreso] INT NOT NULL,
    [FechaHora] DATETIME NOT NULL,
    CONSTRAINT [PK_LogIngresoPorPuesto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LogIngresoPorPuesto_Recorrido] FOREIGN KEY([Recorrido_Id])
        REFERENCES [dbo].[Recorrido] ([Id])
        ON DELETE CASCADE,
    CONSTRAINT [FK_LogIngresoPorPuesto_PuestoDeTrabajo] FOREIGN KEY([PuestoDeTrabajo_Id])
        REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_LogIngresoPorPuesto_Recorrido_Id] 
    ON [dbo].[LogIngresoPorPuesto] ([Recorrido_Id]);

GO
CREATE NONCLUSTERED INDEX [IX_LogIngresoPorPuesto_PuestoDeTrabajo_Id] 
    ON [dbo].[LogIngresoPorPuesto] ([PuestoDeTrabajo_Id]);

GO
CREATE NONCLUSTERED INDEX [IX_LogIngresoPorPuesto_FechaHora] 
    ON [dbo].[LogIngresoPorPuesto] ([FechaHora] DESC);