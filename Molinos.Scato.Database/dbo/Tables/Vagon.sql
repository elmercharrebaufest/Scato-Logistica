CREATE TABLE [dbo].[Vagon] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[Caracteristica]      NVARCHAR (100) NULL,
	[IdMuestra] NVARCHAR (100) NULL,
	[KilosNetosSecos] INT NULL,
	[Numero] NVARCHAR (100) NULL,
	[MuestreoPesajeVagonFerroviarioTransmisionAMonsanto_Id] int NOT NULL,

    CONSTRAINT [PK_dbo.Vagon] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Vagon_dbo.MuestreoPesajeVagonFerroviarioTransmisionAMonsanto_MuestreoPesajeVagonFerroviarioTransmisionAMonsanto_Id] FOREIGN KEY ([MuestreoPesajeVagonFerroviarioTransmisionAMonsanto_Id]) REFERENCES [dbo].[MuestreoPesajeVagonFerroviarioTransmisionAMonsanto] ([Id])
);
