CREATE TABLE [dbo].[ExcepcionAlControlProveedor] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [FechaDesde]        DATETIME     NOT NULL,
    [FechaHasta]        DATETIME     NOT NULL,
    [FechaDeCarga]      DATETIME     NOT NULL,
    [Usuario]           VARCHAR (20) NOT NULL,
    [Proveedor_Id]      INT          NOT NULL,
    [Material_Id]       INT          NOT NULL,
    [Centro_Id]         INT          NOT NULL,
    [Motivo]            INT          DEFAULT ((0)) NOT NULL,
    [CentroDestino_Id]  INT          NULL,
    [ClienteDestino_Id] INT          NULL,
    [TipoDestino]       INT          DEFAULT ((0)) NOT NULL,
    [ProveedorDestino_Id] INT NULL, 
    CONSTRAINT [PK_dbo.ExcepcionAlControlProveedor] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Centro_CentroDestino_Id] FOREIGN KEY ([CentroDestino_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Cliente_ClienteDestino_Id] FOREIGN KEY ([ClienteDestino_Id]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControlProveedor_dbo.Proveedor_ProveedorDestino_Id] FOREIGN KEY ([ProveedorDestino_Id]) REFERENCES [dbo].[Proveedor] ([Id])
);

