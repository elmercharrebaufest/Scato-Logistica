CREATE TABLE [dbo].[MuestreoPesajeVagonFerroviarioTransmisionAMonsanto] (
    [Id]               INT NOT NULL,
	[CuitLaboratorio] NVARCHAR (100) NULL,
	[FechaDeDescarga] DATETIME NULL,
	[NroCartaPorte] BIGINT NULL,
	
	CONSTRAINT [PK_dbo.MuestreoPesajeVagonFerroviarioTransmisionAMonsanto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MuestreoPesajeVagonFerroviarioTransmisionAMonsanto_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);
GO
