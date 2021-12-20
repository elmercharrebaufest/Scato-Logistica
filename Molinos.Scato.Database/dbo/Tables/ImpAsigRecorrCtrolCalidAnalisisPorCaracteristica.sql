CREATE TABLE [dbo].[ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica] (
    [ImpAsigRecorrCtrolCalid_Id]      INT NOT NULL,
    [AnalisisPorCaracteristica_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica] PRIMARY KEY CLUSTERED ([ImpAsigRecorrCtrolCalid_Id] ASC, [AnalisisPorCaracteristica_Id] ASC),
    CONSTRAINT [FK_dbo.ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica_dbo.AnalisisPorCaracteristica_AnalisisPorCaracteristica_Id] FOREIGN KEY ([AnalisisPorCaracteristica_Id]) REFERENCES [dbo].[AnalisisPorCaracteristica] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica_dbo.ImpAsigRecorrCtrolCalid_ImpAsigRecorrCtrolCalid_Id] FOREIGN KEY ([ImpAsigRecorrCtrolCalid_Id]) REFERENCES [dbo].[ImpAsigRecorrCtrolCalid] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ImpAsigRecorrCtrolCalid_Id]
    ON [dbo].[ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica]([ImpAsigRecorrCtrolCalid_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AnalisisPorCaracteristica_Id]
    ON [dbo].[ImpAsigRecorrCtrolCalidAnalisisPorCaracteristica]([AnalisisPorCaracteristica_Id] ASC);
