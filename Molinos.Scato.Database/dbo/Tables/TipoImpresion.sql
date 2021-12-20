CREATE TABLE [dbo].[TipoImpresion] (
    [Id]               INT NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.TipoImpresion] PRIMARY KEY CLUSTERED ([Id] ASC)
);
