CREATE TABLE [dbo].[RecorridoTasaMunicipal]
(
    [Id] INT NOT NULL,
    [Exceptuado] BIT NOT NULL DEFAULT 0,
    [MotivoExceptuado] NVARCHAR(500) NULL,
    CONSTRAINT [PK_dbo.RecorridoTasaMunicipal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.RecorridoTasaMunicipal_dbo.Recorrido_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
)