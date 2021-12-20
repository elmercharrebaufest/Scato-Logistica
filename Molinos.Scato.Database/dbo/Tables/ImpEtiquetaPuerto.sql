CREATE TABLE [dbo].[ImpEtiquetaPuerto] (
    [Id]                   INT IDENTITY (1, 1) NOT NULL,
	[Vapor]	       NVARCHAR(100)  NULL,
    [Cargador]            NVARCHAR (100) NULL, 
    [Mercaderia] NVARCHAR(300) NULL, 
    [Destino] NVARCHAR(100) NULL, 
    [Kg] NVARCHAR(100) NULL, 
    [NumeroLote] NVARCHAR(100) NULL, 
    [Bodega] NVARCHAR(100) NULL, 
    [Control] NVARCHAR(100) NULL, 
    [Fecha] DATETIME NULL,
    [Usuario_Id] INT NOT NULL,
    [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE()
    CONSTRAINT [PK_ImpEtiquetaPuerto] PRIMARY KEY ([Id]),
  )