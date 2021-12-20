CREATE TABLE [dbo].[TipoComercial] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]        NVARCHAR (40) NOT NULL,
	[CodigoSap]			 NVARCHAR (3) NOT NULL,
    [Sentido]            CHAR(1)  NOT NULL,
    [PesoEsperado]       INT            NULL,
    [ToleranciaDifPesoE] INT            NOT NULL,
    [UsaBinPallet]       BIT            NOT NULL,
    [ValidaPatente]      BIT            NOT NULL,
	[TransportistaEsProveedor]		 BIT            NOT NULL default 0,
    [PesoMaximoDocumentoIngreso] INT NULL , 
    [NoRechazaEnCalado] BIT NOT NULL DEFAULT 0, 
    [EsParaUva] INT NOT NULL DEFAULT 0, 
    [ValidaStockEPA] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.TipoComercial] PRIMARY KEY CLUSTERED ([Id] ASC)
);

