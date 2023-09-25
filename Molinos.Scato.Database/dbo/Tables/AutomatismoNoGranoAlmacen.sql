CREATE TABLE [dbo].[AutomatismoNoGranoAlmacen] (
    [AutomatismoNoGrano_Id]     INT NOT NULL,
    [Almacen_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.AutomatismoNoGranoAlmacen] PRIMARY KEY CLUSTERED ([AutomatismoNoGrano_Id] ASC, [Almacen_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AutomatismoNoGranoAlmacen_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AutomatismoNoGranoAlmacen_dbo.AutomatismoNoGrano_AutomatismoNoGrano_Id] FOREIGN KEY ([AutomatismoNoGrano_Id]) REFERENCES [dbo].[AutomatismoNoGrano] ([Id]) ON DELETE CASCADE
);