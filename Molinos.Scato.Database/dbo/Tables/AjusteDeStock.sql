CREATE TABLE [dbo].[AjusteDeStock] (
    [Id]                      INT             IDENTITY (1, 1) NOT NULL,
    [TipoComprobanteOncca_Id] INT             DEFAULT ((1)) NOT NULL,
    [NumeroDocumentoIngreso]  NVARCHAR (40)   NOT NULL,
    [Fecha]                   DATETIME        NOT NULL,
    [Material_Id]             INT             NOT NULL,
    [PesoBrutoIngreso]        DECIMAL (18, 2) NOT NULL,
    [PesoNetoIngreso]         DECIMAL (18, 2) NOT NULL,
    [PesoNetoEgreso]          DECIMAL (18, 2) NOT NULL,
    [Centro_Id]               INT             NOT NULL,
    [Observaciones]           NVARCHAR (30)   NULL,
    [NumeroCTG]               NVARCHAR (40)   NOT NULL DEFAULT '',
    CONSTRAINT [PK_dbo.AjusteDeStock] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AjusteDeStock_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.AjusteDeStock_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.AjusteDeStock_dbo.TipoComprobanteOncca_TipoComprobanteOncca_Id] FOREIGN KEY ([TipoComprobanteOncca_Id]) REFERENCES [dbo].[TipoComprobanteOncca] ([Id])
);

