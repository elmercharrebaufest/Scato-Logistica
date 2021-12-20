CREATE TABLE [dbo].[ImpAsigRecorrCtrolCalidCaladoPorCaracteristica] (
    [ImpAsigRecorrCtrolCalid_Id]      INT NOT NULL,
    [CaladoPorCaracteristica_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpAsigRecorrCtrolCalidCaladoPorCaracteristica] PRIMARY KEY CLUSTERED ([ImpAsigRecorrCtrolCalid_Id] ASC, [CaladoPorCaracteristica_Id] ASC),
    CONSTRAINT [FK_dbo.ImpAsigRecorrCtrolCalidCaladoPorCaracteristica_dbo.CaladoPorCaracteristica_CaladoPorCaracteristica_Id] FOREIGN KEY ([CaladoPorCaracteristica_Id]) REFERENCES [dbo].[CaladoPorCaracteristica] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ImpAsigRecorrCtrolCalidCaladoPorCaracteristica_dbo.ImpAsigRecorrCtrolCalid_ImpAsigRecorrCtrolCalid_Id] FOREIGN KEY ([ImpAsigRecorrCtrolCalid_Id]) REFERENCES [dbo].[ImpAsigRecorrCtrolCalid] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ImpAsigRecorrCtrolCalid_Id]
    ON [dbo].[ImpAsigRecorrCtrolCalidCaladoPorCaracteristica]([ImpAsigRecorrCtrolCalid_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CaladoPorCaracteristica_Id]
    ON [dbo].[ImpAsigRecorrCtrolCalidCaladoPorCaracteristica]([CaladoPorCaracteristica_Id] ASC);
