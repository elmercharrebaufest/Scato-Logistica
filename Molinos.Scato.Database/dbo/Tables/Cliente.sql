CREATE TABLE [dbo].[Cliente] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (100) NOT NULL,
    [Cuit] NVARCHAR(13) NULL, 
	[CodigoSap]  NVARCHAR(10) NULL,
    [Activo] BIT NOT NULL DEFAULT 1, 
	[Direccion]         NVARCHAR (150)  NULL,
	[Localidad]         NVARCHAR (150)  NULL,
	[Provincia]         NVARCHAR (150)  NULL,
    [Bloqueado] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.Cliente] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT UK_Cliente_CodigoSap UNIQUE (CodigoSap)
);

