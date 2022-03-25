CREATE TABLE [dbo].[PinchazosPorCalada] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [TipoPinchazo]      INT            NOT NULL,
    [FechaModificacion] DATETIME       NOT NULL,
    [Usuario_Id]        INT            NOT NULL,
    [Centro_Id]         INT            NOT NULL,
    [Motivo]            NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.PinchazosPorCalada] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PinchazosPorCalada_dbo.Centroo_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.PinchazosPorCalada_dbo.Usuario_Usuario_Id] FOREIGN KEY ([Usuario_Id]) REFERENCES [dbo].[Usuario] ([Id]) ON DELETE CASCADE
);



GO

CREATE NONCLUSTERED INDEX [IX_Usuario_Id]
    ON [dbo].[PinchazosPorCalada]([Usuario_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

