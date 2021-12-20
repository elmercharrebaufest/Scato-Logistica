CREATE TABLE [dbo].[TarjetaBloqueada] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Numero]      NVARCHAR (10)  NOT NULL,
    [Motivo]      NVARCHAR (30)  NOT NULL,
    [Centro_Id]        INT            NULL,
    CONSTRAINT [PK_dbo.TarjetaBloqueada] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TarjetaBloqueada_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[TarjetaBloqueada]([Centro_Id] ASC);

