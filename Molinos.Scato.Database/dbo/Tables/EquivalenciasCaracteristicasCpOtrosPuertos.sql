CREATE TABLE [dbo].[EquivalenciasCaracteristicasCpOtrosPuertos] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Material_Id]   INT           NOT NULL,
    [CodigoExterno] NVARCHAR (50) NOT NULL,
    [CodigoSap]     NVARCHAR (20) NOT NULL,
    [EsHumedad]     BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


