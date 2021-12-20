CREATE TABLE [dbo].[Descuento] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [ValorHasta]        DECIMAL(18, 2)  NOT NULL,
	[PorcentajeDescuento]	 DECIMAL(18, 2)	NOT NULL,
	[CaracteristicaDeCalidad_Id] INT  NOT NULL,
	[PorcentajeEnvioCamaraAuditoria]	 DECIMAL(18, 2)	NOT NULL default 0,
	[MercadoATermino]	 bit	NOT NULL default 0,
    CONSTRAINT [PK_dbo.Descuento] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Descuento_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id])
);
