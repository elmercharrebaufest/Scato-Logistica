CREATE TABLE [dbo].[ImpInformeDeRecepcion] (
    [Id]                    INT      NOT NULL,
	[Fecha]			DATETIME		 NOT NULL, 
    [NumeroInforme]			NVARCHAR (50)    NOT NULL,
	[NumeroPedido]			NVARCHAR (50)    NOT NULL,
    [NumeroRemito]			NVARCHAR (50)    NULL,
    [Observaciones]		NVARCHAR (50)    NULL,
    [Estado]	NVARCHAR (50)    NOT NULL,
   	[Proveedor] NVARCHAR(50) NOT NULL, 
	CONSTRAINT [PK_dbo.ImpInformeDeRecepcion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ImpInformeDeRecepcion_dbo.ImpId] FOREIGN KEY ([Id]) REFERENCES [dbo].[Impresion] ([Id]) ON DELETE CASCADE
);