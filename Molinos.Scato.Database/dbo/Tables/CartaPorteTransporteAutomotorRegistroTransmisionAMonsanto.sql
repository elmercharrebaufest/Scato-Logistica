CREATE TABLE [dbo].[CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto] (
    [Id]               INT NOT NULL,
	CodigoLocalidadDestino BIGINT NULL,
	NumeroPlantaDestino BIGINT NULL,

	CodigoBiotecnologiaDeclarada NVARCHAR (100) NULL,
	CodigoEspecie NVARCHAR (100) NULL,
	CodigoLocalidadProcedencia BIGINT NULL,
	Establecimiento NVARCHAR (100) NULL,

	DestinatarioCuit NVARCHAR (100) NULL,

	DestinoCuit NVARCHAR (100) NULL,

	RemitenteComercialCuit NVARCHAR (100) NULL,
	RemitenteComercial NVARCHAR (100) NULL,
	
	TitularCuit NVARCHAR (100) NULL,
	Titular NVARCHAR (100) NULL,

	Ctg NVARCHAR (100) NULL,
	NumeroCartaPorte BIGINT NULL,

	CONSTRAINT [PK_dbo.CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto_dbo.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);
GO
