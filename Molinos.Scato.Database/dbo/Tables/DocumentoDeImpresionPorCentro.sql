CREATE TABLE [dbo].[DocumentoDeImpresionPorCentro] (
    [Id]                      INT IDENTITY (1, 1) NOT NULL,
    [DocumentoDeImpresion_Id] INT NOT NULL,
    [FormatoDeImpresion_Id]   INT NULL,
    [Impresora_Id]            INT NOT NULL,
    [Centro_Id]               INT NOT NULL,
    [PuestoDeTrabajo_Id]      INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.DocumentoDeImpresionPorCentro_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.DocumentoDeImpresionPorCentro_dbo.DocumentoDeImpresion_DocumentoDeImpresion_Id] FOREIGN KEY ([DocumentoDeImpresion_Id]) REFERENCES [dbo].[DocumentoDeImpresion] ([Id]),
    CONSTRAINT [FK_dbo.DocumentoDeImpresionPorCentro_dbo.FormatoDeImpresion_FormatoDeImpresion_Id] FOREIGN KEY ([FormatoDeImpresion_Id]) REFERENCES [dbo].[FormatoDeImpresion] ([Id]),
    CONSTRAINT [FK_dbo.DocumentoDeImpresionPorCentro_dbo.Impresora_Impresora_Id] FOREIGN KEY ([Impresora_Id]) REFERENCES [dbo].[Impresora] ([Id]),
    CONSTRAINT [FK_dbo.DocumentoDeImpresionPorCentro_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);


