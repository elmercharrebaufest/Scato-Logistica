CREATE TABLE [dbo].[PuestoDeTrabajoTarjetaSupervisor] (
    [TarjetaSupervisor_Id]      INT NOT NULL,
    [PuestoDeTrabajo_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.PuestoDeTrabajoTarjetaSupervisor] PRIMARY KEY CLUSTERED (TarjetaSupervisor_Id ASC, [PuestoDeTrabajo_Id] ASC),
    CONSTRAINT [FK_dbo.PuestoDeTrabajoTarjetaSupervisor_dbo.TarjetaSupervisor_TarjetaSupervisor_Id] FOREIGN KEY (TarjetaSupervisor_Id) REFERENCES [dbo]. [TarjetaSupervisor] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.PuestoDeTrabajoTarjetaSupervisor_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TarjetaSupervisor_Id]
    ON [dbo].[PuestoDeTrabajoTarjetaSupervisor]([TarjetaSupervisor_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[PuestoDeTrabajoTarjetaSupervisor]([PuestoDeTrabajo_Id] ASC);

