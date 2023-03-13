CREATE TABLE [dbo].[MaterialPuerto] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (MAX) NULL,
    [DescripcionCorta] NVARCHAR (50)  NULL,
    [CodigoSap]        NVARCHAR (50)  NULL,
    [Almacen_Id]       INT            NULL,
    [EsLiquido]        BIT            DEFAULT ((0)) NOT NULL,
    [Color]            NVARCHAR (7)   DEFAULT ('#000000') NULL,
    CONSTRAINT [PK_MaterialPuerto] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MaterialPuerto_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id])
);

