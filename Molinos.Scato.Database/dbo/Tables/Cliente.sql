CREATE TABLE [dbo].[Cliente] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion] NVARCHAR (100) NOT NULL,
    [Cuit]        NVARCHAR (13)  NULL,
    [CodigoSap]   NVARCHAR (10)  NULL,
    [Activo]      BIT            DEFAULT ((1)) NOT NULL,
    [Direccion]   NVARCHAR (150) NULL,
    [Localidad]   NVARCHAR (150) NULL,
    [Provincia]   NVARCHAR (150) NULL,
    [Bloqueado]   BIT            DEFAULT ((0)) NOT NULL,
    [EsClienteProvisorio] BIT NOT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_dbo.Cliente] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [UK_Cliente_CodigoSap] UNIQUE NONCLUSTERED ([CodigoSap] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



