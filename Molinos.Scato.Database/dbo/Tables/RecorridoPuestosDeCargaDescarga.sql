CREATE TABLE [dbo].[RecorridoPuestosDeCargaDescarga] (
    [Recorrido_Id]      INT NOT NULL,
    [PuestosDeCargaDescarga_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.RecorridoPuestosDeCargaDescarga] PRIMARY KEY CLUSTERED ([Recorrido_Id] ASC, [PuestosDeCargaDescarga_Id] ASC),
    CONSTRAINT [FK_dbo.RecorridoPuestosDeCargaDescarga_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RecorridoPuestosDeCargaDescarga_dbo.PuestosDeCargaDescarga_PuestosDeCargaDescarga_Id] FOREIGN KEY ([PuestosDeCargaDescarga_Id]) REFERENCES [dbo].[PuestosDeCargaDescarga] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[RecorridoPuestosDeCargaDescarga]([Recorrido_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PuestosDeCargaDescarga_Id]
    ON [dbo].[RecorridoPuestosDeCargaDescarga]([PuestosDeCargaDescarga_Id] ASC);

