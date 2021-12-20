CREATE TABLE [dbo].[Chofer] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Apellido]                  NVARCHAR (20) NOT NULL,
    [Nombre]                    NVARCHAR (20) NOT NULL,
    [NumeroDeDocumento]         NVARCHAR (10) NOT NULL,
    [Cuil]                      NVARCHAR (13) NOT NULL,
    [TipoDocumentoIdentidad_Id] INT            NOT NULL,
    CONSTRAINT [PK_dbo.Chofer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Chofer_dbo.TipoDocumentoIdentidad_TipoDocumentoIdentidad_Id] FOREIGN KEY ([TipoDocumentoIdentidad_Id]) REFERENCES [dbo].[TipoDocumentoIdentidad] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_TipoDocumentoIdentidad_Id]
    ON [dbo].[Chofer]([TipoDocumentoIdentidad_Id] ASC);
