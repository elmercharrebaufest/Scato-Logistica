CREATE TABLE [dbo].[MensajeCartelLed]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Codigo] NVARCHAR (100) NOT NULL,
	[Orden] INT NOT NULL,
	[Mensaje] NVARCHAR(100) NOT NULL,
	[Programa] NVARCHAR(10) NOT NULL,
	[Trama] NVARCHAR(10) NOT NULL,
	[Variable] NVARCHAR(10) NOT NULL,
	[SegundosDeEspera] INT NOT NULL DEFAULT 0,
	[DescripcionFormatoMensaje] NVARCHAR(500) NULL,
	[Habilitado] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_dbo.MensajeCartelLed] PRIMARY KEY CLUSTERED ([Id] ASC),
)
