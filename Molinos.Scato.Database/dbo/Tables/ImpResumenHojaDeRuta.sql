CREATE TABLE [dbo].[ImpResumenHojaDeRuta] (
    [Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.ImpResumenHojaDeRuta] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ImpResumenHojaDeRuta_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);

