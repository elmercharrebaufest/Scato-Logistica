CREATE TABLE [dbo].[TipoVehiculoBodega] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]                    NVARCHAR (35) NOT NULL,
	[DescripcionCorta]                    NVARCHAR (35) NULL,
    CONSTRAINT [PK_dbo.TipoVehiculoBodega] PRIMARY KEY CLUSTERED ([Id] ASC),
);