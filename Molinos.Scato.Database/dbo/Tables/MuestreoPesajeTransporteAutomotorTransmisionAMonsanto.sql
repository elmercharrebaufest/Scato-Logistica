CREATE TABLE [dbo].[MuestreoPesajeTransporteAutomotorTransmisionAMonsanto] (
    [Id]               INT NOT NULL,
	[CuitLaboratorio] NVARCHAR (100) NULL,
	[FechaDeDescarga] DATETIME NULL,
	[IdMuestra] NVARCHAR (100) NULL,
	[KilosNetosSecos] INT NULL,
	[NroCartaPorte] BIGINT NULL,
	
	CONSTRAINT [PK_dbo.MuestreoPesajeTransporteAutomotorTransmisionAMonsanto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MuestreoPesajeTransporteAutomotorTransmisionAMonsanto_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);
GO
