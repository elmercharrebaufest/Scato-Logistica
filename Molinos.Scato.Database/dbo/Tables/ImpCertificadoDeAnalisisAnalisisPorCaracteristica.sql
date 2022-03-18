CREATE TABLE [dbo].[ImpCertificadoDeAnalisisAnalisisPorCaracteristica] (
    [ImpCertificadoDeAnalisis_Id]  INT NOT NULL,
    [AnalisisPorCaracteristica_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpCertificadoDeAnalisisAnalisisPorCaracteristica] PRIMARY KEY CLUSTERED ([ImpCertificadoDeAnalisis_Id] ASC, [AnalisisPorCaracteristica_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ImpCertificadoDeAnalisisAnalisisPorCaracteristica_dbo.AnalisisPorCaracteristica_AnalisisPorCaracteristica_Id] FOREIGN KEY ([AnalisisPorCaracteristica_Id]) REFERENCES [dbo].[AnalisisPorCaracteristica] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ImpCertificadoDeAnalisisAnalisisPorCaracteristica_dbo.ImpCertificadoDeAnalisis_ImpCertificadoDeAnalisis_Id] FOREIGN KEY ([ImpCertificadoDeAnalisis_Id]) REFERENCES [dbo].[ImpCertificadoDeAnalisis] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_ImpCertificadoDeAnalisis_Id]
    ON [dbo].[ImpCertificadoDeAnalisisAnalisisPorCaracteristica]([ImpCertificadoDeAnalisis_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_AnalisisPorCaracteristica_Id]
    ON [dbo].[ImpCertificadoDeAnalisisAnalisisPorCaracteristica]([AnalisisPorCaracteristica_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


