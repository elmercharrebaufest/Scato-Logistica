CREATE TABLE [dbo].[DocumentoDeImpresion] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Codigo]           NVARCHAR (40) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    [DescripcionCorta] NVARCHAR (40) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


