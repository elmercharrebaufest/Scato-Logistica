CREATE TABLE [dbo].[NirsModificacionModalidad] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Modalidad] NVARCHAR(10) NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [Motivo] NVARCHAR(500) NOT NULL,
	[NombreUsuarioResponsable] NVARCHAR (MAX) NOT NULL,
    [Nirs_Id] INT NOT NULL,
	CONSTRAINT [PK_dbo.NirsModificacionModalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.NirsModificacionModalidad_dbo.Nirs_Nirs_Id] FOREIGN KEY ([Nirs_Id]) REFERENCES [dbo].[Nirs] ([Id]) ON DELETE CASCADE,
);
GO
CREATE NONCLUSTERED INDEX [IX_Nirs_Id]
    ON [dbo].[NirsModificacionModalidad]([Nirs_Id] ASC);