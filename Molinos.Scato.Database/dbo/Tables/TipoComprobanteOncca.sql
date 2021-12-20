CREATE TABLE [dbo].[TipoComprobanteOncca] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    [CodigoOncca]      NVARCHAR (20) NOT NULL,
	CONSTRAINT [PK_dbo.TipoComprobanteOncca] PRIMARY KEY CLUSTERED ([Id] ASC)
);