CREATE TABLE [dbo].[AsignacionDeRecorrido] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[FechaDesde]         DATETIME NOT NULL,
    [FechaHasta]         DATETIME NOT NULL,
    [MaterialPorCentro_Id] INT            NOT NULL,
    [Calidad_Id] INT             NULL,
	[Calle_Id]    INT	 NOT NULL,
	[BalanzaBruto_Id]    INT	 NULL,
	[BalanzaTara_Id]    INT	  NULL,
	[AlmacenDestino_Id]    INT	 NOT NULL,
    [Centro_Id] INT NOT NULL, 
	[Workflow_Id] INT NULL, 
    CONSTRAINT [PK_dbo.AsignacionDeRecorrido] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.MaterialPorCentro_MaterialPorCentro_Id] FOREIGN KEY ([MaterialPorCentro_Id]) REFERENCES [dbo].[MaterialPorCentro] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.CalidadMaterial_Calidad_Id] FOREIGN KEY ([Calidad_Id]) REFERENCES [dbo].[CalidadMaterial] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Calle_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Balanza_BalanzaBruto_Id] FOREIGN KEY ([BalanzaBruto_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Balanza_BalanzaTara_Id] FOREIGN KEY ([BalanzaTara_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Almacen_AlmacenDestino_Id] FOREIGN KEY ([AlmacenDestino_Id]) REFERENCES [dbo].[Almacen] ([Id]),
    CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [FK_dbo.AsignacionDeRecorrido_dbo.Workflow_workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id]),
    
);
GO

CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[AsignacionDeRecorrido]([MaterialPorCentro_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Calidad_Id]
    ON [dbo].[AsignacionDeRecorrido]([Calidad_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Calle_Id]
    ON [dbo].[AsignacionDeRecorrido]([Calle_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_BalanzaBruto_Id]
    ON [dbo].[AsignacionDeRecorrido]([BalanzaBruto_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_BalanzaTara_Id]
    ON [dbo].[AsignacionDeRecorrido]([BalanzaTara_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AlmacenDestino_Id]
    ON [dbo].[AsignacionDeRecorrido]([AlmacenDestino_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[AsignacionDeRecorrido]([Centro_Id] ASC);
GO
