CREATE TABLE [dbo].[ActividadPorDispositivo] (
    [Id]                   INT IDENTITY (1, 1) NOT NULL,
	[Workflow_Id]	       INT  NOT NULL,
    [Actividad]            NVARCHAR (40) NOT NULL,
    [Salida]               NVARCHAR (200) NULL,
	[VideoCamara]			   NVARCHAR (50) NULL,
	[VideoCamaraDirectorio]	   NVARCHAR (100) NULL,
	[PuestoDeTrabajo_Id]   INT  NOT NULL,
	[Entrada]               NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.ActividadPorDispositivo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ActividadPorDispositivo_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.ActividadPorDispositivo_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[ActividadPorDispositivo]([PuestoDeTrabajo_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Workflow_Id]
    ON [dbo].[ActividadPorDispositivo]([Workflow_Id] ASC);
