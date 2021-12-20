CREATE TABLE [dbo].[AsignacionDeRecorridoPuestosDeCargaDescarga] (
    [AsignacionDeRecorrido_Id]      INT NOT NULL,
    [PuestosDeCargaDescarga_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.AsignacionDeRecorridoPuestosDeCargaDescarga] PRIMARY KEY CLUSTERED ([AsignacionDeRecorrido_Id] ASC, [PuestosDeCargaDescarga_Id] ASC),
    CONSTRAINT [FK_dbo.AsignacionDeRecorridoPuestosDeCargaDescarga_dbo.AsignacionDeRecorrido_AsignacionDeRecorrido_Id] FOREIGN KEY ([AsignacionDeRecorrido_Id]) REFERENCES [dbo].[AsignacionDeRecorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AsignacionDeRecorridoPuestosDeCargaDescarga_dbo.PuestosDeCargaDescarga_PuestosDeCargaDescarga_Id] FOREIGN KEY ([PuestosDeCargaDescarga_Id]) REFERENCES [dbo].[PuestosDeCargaDescarga] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[AsignacionDeRecorridoPuestosDeCargaDescarga]([AsignacionDeRecorrido_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PuestosDeCargaDescarga_Id]
    ON [dbo].[AsignacionDeRecorridoPuestosDeCargaDescarga]([PuestosDeCargaDescarga_Id] ASC);

