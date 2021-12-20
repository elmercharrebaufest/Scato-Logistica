CREATE TABLE [dbo].[TipoMicromuestra]
(
    [Id]               INT NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.TipoMicromuestra] PRIMARY KEY CLUSTERED ([Id] ASC)
)
