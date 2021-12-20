CREATE TABLE [dbo].[AnalisisObligatorioPuestoDeTrabajo] (
    [AnalisisObligatorio_Id]      INT NOT NULL,
    [PuestoDeTrabajo_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.AnalisisObligatorioPuestoDeTrabajo] PRIMARY KEY CLUSTERED ([AnalisisObligatorio_Id] ASC, [PuestoDeTrabajo_Id] ASC),
    CONSTRAINT [FK_dbo.AnalisisObligatorioPuestoDeTrabajo_dbo.AnalisisObligatorio_AnalisisObligatorio_Id] FOREIGN KEY (AnalisisObligatorio_Id) REFERENCES [dbo]. [AnalisisObligatorio] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AnalisisObligatorioPuestoDeTrabajo_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AnalisisObligatorio_Id]
    ON [dbo].[AnalisisObligatorioPuestoDeTrabajo]([AnalisisObligatorio_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[AnalisisObligatorioPuestoDeTrabajo]([PuestoDeTrabajo_Id] ASC);

