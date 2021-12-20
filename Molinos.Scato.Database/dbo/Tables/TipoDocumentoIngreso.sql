CREATE TABLE [dbo].[TipoDocumentoIngreso] (
    [Id]               INT NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.TipoDocumentoIngreso] PRIMARY KEY CLUSTERED ([Id] ASC)
);
