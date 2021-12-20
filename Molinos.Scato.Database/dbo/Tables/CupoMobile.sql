CREATE TABLE [dbo].[CupoMobile] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
    [Otorgados]      INT  NOT NULL,
	[ArribadosDia]      INT  NOT NULL,
	[ArribadosVencidos]      INT  NOT NULL,
	[ArribadosFuturos]      INT  NOT NULL,
	[Descargados]      INT  NOT NULL,
	[Pendiente]      INT  NOT NULL,
	[SinRecorrido]      INT  NOT NULL,
	[Excedente]      INT  NOT NULL,
	[Material]      nvarchar(100)  NOT NULL,
	[Orden]      INT  NOT NULL,
	[SinCupo]      INT  NOT NULL,
	[DescargadosTodos]      INT  NOT NULL,
	[CentroId]      INT  NOT NULL,
	[FechaActualizacion] datetime NOT NULL
	    CONSTRAINT [PK_dbo.CupoMobile] PRIMARY KEY CLUSTERED ([Id] ASC), 
);