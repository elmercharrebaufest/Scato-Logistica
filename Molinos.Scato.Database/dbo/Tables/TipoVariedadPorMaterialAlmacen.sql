CREATE TABLE [dbo].[TipoVariedadPorMaterialAlmacen](
	[TipoVariedadPorMaterial_Id] [int] NOT NULL,
	[Almacen_Id] [int] NOT NULL,
	CONSTRAINT [PK_TipoVariedadPorMaterialAlmacen] PRIMARY KEY CLUSTERED 
	(
		[TipoVariedadPorMaterial_Id] ASC,
		[Almacen_Id] ASC
	),
	CONSTRAINT [FK_TipoVariedadPorMaterialAlmacen_TipoVariedadPorMaterial] FOREIGN KEY([TipoVariedadPorMaterial_Id])
	REFERENCES [dbo].[TipoVariedadPorMaterial] ([Id]),
	CONSTRAINT [FK_TipoVariedadPorMaterialAlmacen_Almacen] FOREIGN KEY([Almacen_Id])
	REFERENCES [dbo].[Almacen] ([Id])
) ON [PRIMARY]