CREATE TABLE [dbo].[Firma]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Descripcion] NVARCHAR(40) NOT NULL, 
    [RazonSocial] NVARCHAR(35) NULL, 
    [CodigoSAP] NVARCHAR(20) NULL, 
    [Cuit] NVARCHAR(13) NULL, 
    [Logo] VARBINARY(MAX) NULL,
	[DescripcionCorta] NVARCHAR(10) NULL, 
    [IngBrutosConvMultilateral] NVARCHAR(20) NULL, 
    [Direccion] NVARCHAR(50) NULL, 
    [Ciudad] NVARCHAR(40) NULL, 
    [FechaDeInicio] NVARCHAR(10) NULL, 
    [Favicon] VARBINARY(MAX) NULL, 
    CONSTRAINT [PK_dbo.Firma] PRIMARY KEY CLUSTERED ([Id] ASC)
)
