CREATE TABLE [dbo].[TipoDeWorkflow] (
    [Id]               INT           NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.TipoDeWorkflow] PRIMARY KEY CLUSTERED ([Id] ASC)
);
