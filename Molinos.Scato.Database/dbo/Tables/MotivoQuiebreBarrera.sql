CREATE TABLE [dbo].[MotivoQuiebreBarrera] (
    [Id]                        INT				IDENTITY (1, 1) NOT NULL,
    [PuestoTrabajo_Id]          INT				NOT NULL,
    [Fecha]						DATETIME		NOT NULL, 
	[Apertura]					BIT		NOT NULL default 1, 
    [Motivo]					NVARCHAR(250)	NULL, 
    [Patente] NCHAR(10) NULL, 
    [Transportista_Id] INT NULL, 
	[FileName]					NVARCHAR(50)	NULL,
    CONSTRAINT [PK_dbo.MotivoQuiebreBarrera] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.MotivoQuiebreBarrera_dbo.MotivoQuiebreBarrera_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.MotivoQuiebreBarrera_dbo.MotivoQuiebreBarrera_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
);