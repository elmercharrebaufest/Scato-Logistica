CREATE TABLE [dbo].[DocumentoExterno] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [NumeroDeDocumento]    NVARCHAR (15) NOT NULL,
    [TipoDocumentoIngreso] INT           NOT NULL,
    [ArchivoRutaDestino]   VARCHAR (MAX) NULL,
    [ArchivoExtension]     VARCHAR (MAX) NULL,
    [Fecha]                DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.DocumentoExterno] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.DocumentoExterno_dbo.TipoIngresoId_TipoDocumentoIngreso_Id] FOREIGN KEY ([TipoDocumentoIngreso]) REFERENCES [dbo].[TipoDocumentoIngreso] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_TipoDocumentoIngreso_Id]
    ON [dbo].[DocumentoExterno]([TipoDocumentoIngreso] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);




