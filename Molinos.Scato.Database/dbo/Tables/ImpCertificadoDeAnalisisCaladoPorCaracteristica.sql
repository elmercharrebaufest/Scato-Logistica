CREATE TABLE [dbo].[ImpCertificadoDeAnalisisCaladoPorCaracteristica] (
    [ImpCertificadoDeAnalisis_Id] INT NOT NULL,
    [CaladoPorCaracteristica_Id]  INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpCertificadoDeAnalisisCaladoPorCaracteristica] PRIMARY KEY CLUSTERED ([ImpCertificadoDeAnalisis_Id] ASC, [CaladoPorCaracteristica_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ImpCertificadoDeAnalisisCaladoPorCaracteristica_dbo.CaladoPorCaracteristica_CaladoPorCaracteristica_Id] FOREIGN KEY ([CaladoPorCaracteristica_Id]) REFERENCES [dbo].[CaladoPorCaracteristica] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ImpCertificadoDeAnalisisCaladoPorCaracteristica_dbo.ImpCertificadoDeAnalisis_ImpCertificadoDeAnalisis_Id] FOREIGN KEY ([ImpCertificadoDeAnalisis_Id]) REFERENCES [dbo].[ImpCertificadoDeAnalisis] ([Id]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_ImpCertificadoDeAnalisis_Id]
    ON [dbo].[ImpCertificadoDeAnalisisCaladoPorCaracteristica]([ImpCertificadoDeAnalisis_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_CaladoPorCaracteristica_Id]
    ON [dbo].[ImpCertificadoDeAnalisisCaladoPorCaracteristica]([CaladoPorCaracteristica_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


