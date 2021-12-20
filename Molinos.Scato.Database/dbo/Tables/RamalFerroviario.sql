CREATE TABLE [dbo].[RamalFerroviario] (
    [Id]                           INT IDENTITY (1, 1) NOT NULL,
    [CodigoAfip]                   INT NOT NULL,
    [Descripcion]                  NVARCHAR (100) NOT NULL,
    [FechaCreacion]				   DATETIME NOT NULL,
    [Deshabilitada]                BIT NOT NULL default 0,

    CONSTRAINT [PK_dbo.RamalFerroviario] PRIMARY KEY CLUSTERED ([Id] ASC)
);