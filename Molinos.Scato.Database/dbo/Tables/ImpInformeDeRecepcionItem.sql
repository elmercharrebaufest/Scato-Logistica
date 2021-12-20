CREATE TABLE [dbo].[ImpInformeDeRecepcionItem] (
    [Id]                     INT              IDENTITY (1, 1) NOT NULL,
	[ImpInformeDeRecepcion_Id]	INT		NOT NULL,
    [ItemNro] INT NOT NULL,
    [MaterialCodigo] NVARCHAR(50) NOT NULL,
    [MaterialDescripcion] NVARCHAR(50) NOT NULL,
    [UniMed] NVARCHAR(50) NOT NULL,
    [CantidadDescargada] NVARCHAR(50) NOT NULL,
	[Remito] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_dbo.ImpInformeDeRecepcionItem_dbo.ImpInformeDeRecepcion_ImpInformeDeRecepcion_Id] FOREIGN KEY ([ImpInformeDeRecepcion_Id]) REFERENCES [dbo].[ImpInformeDeRecepcion] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [PK_dbo.ImpInformeDeRecepcionItem] PRIMARY KEY CLUSTERED ([Id] ASC),
);