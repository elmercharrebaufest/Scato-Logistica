CREATE TABLE [dbo].[CategoriaVehiculo] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [Patente]          VARCHAR (10) NOT NULL,
    [PatenteAcoplado]  VARCHAR (10) NULL,
    [PatenteAcoplado2] VARCHAR (10) NULL,
    [TipoVehiculo]     INT          NOT NULL,
    CONSTRAINT [PK_dbo.CategoriaVehiculo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


