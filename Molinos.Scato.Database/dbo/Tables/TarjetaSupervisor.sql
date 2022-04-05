CREATE TABLE [dbo].[TarjetaSupervisor] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Numero]      NVARCHAR (10) NOT NULL,
    [Descripcion] NVARCHAR (30) NOT NULL,
    [Salida]      INT           NOT NULL,
    [Centro_Id]   INT           NULL,
    CONSTRAINT [PK_dbo.TarjetaSupervisor] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.TarjetaSupervisor_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[TarjetaSupervisor]([Centro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

