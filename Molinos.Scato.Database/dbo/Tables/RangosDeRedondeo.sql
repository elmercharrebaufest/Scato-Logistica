CREATE TABLE [dbo].[RangosDeRedondeo] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [ValorDesde]        DECIMAL(18, 2)  NOT NULL,
    [ValorHasta]        DECIMAL(18, 2)  NOT NULL,
	[ValorRedondeado]	 DECIMAL(18, 2)	NOT NULL,
	[MaterialPorCentro_Id] INT  NOT NULL,
    CONSTRAINT [PK_dbo.RangosDeRedondeo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.RangosDeRedondeo_dbo.MaterialPorCentro_MaterialPorCentro_Id] FOREIGN KEY ([MaterialPorCentro_Id]) REFERENCES [dbo].[MaterialPorCentro] ([Id])
);
