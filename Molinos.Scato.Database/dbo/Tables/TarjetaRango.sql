CREATE TABLE [dbo].[TarjetaRango] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [Codigo]      NVARCHAR (5)  NOT NULL,
    [ValidoDesde]      DATE  NOT NULL,
    [ValidoHasta]      DATE  NOT NULL,
    [RangoDesde]      NVARCHAR (5)  NOT NULL,
    [RangoHasta]      NVARCHAR (5)  NOT NULL,
    [Centro_Id]        INT       NULL,
    CONSTRAINT [PK_dbo.TarjetaRango] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TarjetaRango_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[TarjetaRango]([Centro_Id] ASC);

