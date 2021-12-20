CREATE TABLE [dbo].[VariedadPorVinedo] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[Cosecha]         nvarchar(4) NOT NULL,
    [Variedad_Id]         INT NOT NULL,
	[Vinedo_Id]         INT NOT NULL,
    [Hectareas]  DECIMAL(18, 2) NOT NULL,
	[AvisoCorte]  DECIMAL(18, 2) NOT NULL,
    [TopeHectarea]    DECIMAL(18, 2) NOT NULL,
    CONSTRAINT [PK_dbo.VariedadPorVinedo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.VariedadPorVinedo_dbo.VariedadPorVinedo_Variedad_Id] FOREIGN KEY ([Variedad_Id]) REFERENCES [dbo].[Variedad] ([Id]),
	CONSTRAINT [FK_dbo.VariedadPorVinedo_dbo.VariedadPorVinedo_Vinedo_Id] FOREIGN KEY ([Vinedo_Id]) REFERENCES [dbo].[Vinedo] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Variedad_Id]
    ON [dbo].[VariedadPorVinedo]([Variedad_Id] ASC);


GO