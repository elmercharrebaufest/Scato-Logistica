CREATE TABLE [dbo].[DescargaUnidadItemPedido] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [DescargaUnidad_Id]		INT NOT NULL,
    CONSTRAINT	[FK_dbo.DescargaUnidadItemPedido_dbo.DescargaUnidad_DescargaUnidad_Id] FOREIGN KEY ([DescargaUnidad_Id]) REFERENCES [dbo].[DescargaUnidad] ([Id]) ON DELETE CASCADE,
    [Material_Id]		INT NOT NULL,
	CONSTRAINT	[FK_dbo.DescargaUnidadItemPedido_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    [Ebelp]		NVARCHAR (20) NULL,
	[CantPedido]		NVARCHAR (20) NULL,
	[FecEntrega]		NVARCHAR (20) NULL,
	[PorcentajeExc]		NVARCHAR (20) NULL,
	[Werks]		NVARCHAR (20) NULL,
	CONSTRAINT [PK_dbo.DescargaUnidadItemPedido] PRIMARY KEY CLUSTERED ([Id] ASC)
);
