CREATE TABLE [dbo].[TipoVehiculo] (
    [Id]          INT           NOT NULL,
    [Descripcion] NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_dbo.TipoVehiculo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


