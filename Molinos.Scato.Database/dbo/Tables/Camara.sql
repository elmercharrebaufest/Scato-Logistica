CREATE TABLE [dbo].[Camara] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]        NVARCHAR (30)  NOT NULL,
	[DescripcionCorta]	 NVARCHAR (8)	NOT NULL,
    [CodigoSAP]          NVARCHAR (20)	NOT NULL,
	[FormatoDeArchivo]	 INT  NULL,
	[Email]				 NVARCHAR (200) NULL
    CONSTRAINT [PK_dbo.Camara] PRIMARY KEY CLUSTERED ([Id] ASC),
);
