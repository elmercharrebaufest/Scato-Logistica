CREATE TABLE [dbo].[Descuento] (
    [Id]                             INT             IDENTITY (1, 1) NOT NULL,
    [ValorHasta]                     DECIMAL (18, 2) NOT NULL,
    [PorcentajeDescuento]            DECIMAL (18, 2) NOT NULL,
    [CaracteristicaDeCalidad_Id]     INT             NOT NULL,
    [PorcentajeEnvioCamaraAuditoria] DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [MercadoATermino]                BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Descuento] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Descuento_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id])
);


