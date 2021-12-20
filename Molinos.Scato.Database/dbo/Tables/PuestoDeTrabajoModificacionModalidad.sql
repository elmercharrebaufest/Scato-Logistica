CREATE TABLE [dbo].[PuestoDeTrabajoModificacionModalidad]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
	[Fecha] DATETIME NOT NULL, 
    [Automatico] BIT NOT NULL, 
    [Motivo] NVARCHAR(MAX) NOT NULL,
	[NombreUsuarioResponsable] NVARCHAR (MAX) NOT NULL,
    [PuestoDeTrabajo_Id] INT NOT NULL,
    [Centro_Id] INT NOT NULL DEFAULT 3,
	CONSTRAINT [PK_dbo.PuestoDeTrabajoModificacionModalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.PuestoDeTrabajoModificacionModalidad_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.PuestoDeTrabajoModificacionModalidad_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),

)
GO
