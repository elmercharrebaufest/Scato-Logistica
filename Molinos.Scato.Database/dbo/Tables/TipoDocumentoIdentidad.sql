CREATE TABLE [dbo].[TipoDocumentoIdentidad] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    [DescripcionCorta] NVARCHAR (3) NOT NULL,
	[CodigoSap] NVARCHAR (3) NOT NULL,
    CONSTRAINT [PK_dbo.TipoDocumentoIdentidad] PRIMARY KEY CLUSTERED ([Id] ASC)
);

