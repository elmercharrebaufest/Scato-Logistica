CREATE TABLE [dbo].[Carga](
	[Id] [int] NOT NULL,
	[NumeroBalanza] [nvarchar](50) NOT NULL,
	[Vapor_Id] [int] NOT NULL,
	[Material_Id] [int] NOT NULL,
	[Bodega_Id] [int] NOT NULL,
	[Exportador_Id] [int] NOT NULL,
	[Destino_Id] [int] NOT NULL,
	[PesoProgramado] [int] NULL,
	[ToneladasAW] [int] NULL,
	[FechaInicio] [datetime] NULL,
	[CargaOpuesta_Id] [int] NULL,
	[CargaOpuesta_NumeroBalanza] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Carga] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[NumeroBalanza] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Carga]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.Bodega_Bodega_Id] FOREIGN KEY([Bodega_Id])
REFERENCES [dbo].[Bodega] ([Id])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.Bodega_Bodega_Id]
GO

ALTER TABLE [dbo].[Carga]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.Carga_CargaOpuesta_Id] FOREIGN KEY([CargaOpuesta_Id], [CargaOpuesta_NumeroBalanza])
REFERENCES [dbo].[Carga] ([Id], [NumeroBalanza])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.Carga_CargaOpuesta_Id]
GO

ALTER TABLE [dbo].[Carga]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.Destino_Destino_Id] FOREIGN KEY([Destino_Id])
REFERENCES [dbo].[Destino] ([Id])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.Destino_Destino_Id]
GO

ALTER TABLE [dbo].[Carga]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.Exportador_Exportador_Id] FOREIGN KEY([Exportador_Id])
REFERENCES [dbo].[Exportador] ([Id])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.Exportador_Exportador_Id]
GO

ALTER TABLE [dbo].[Carga]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.MaterialPuerto_Material_Id] FOREIGN KEY([Material_Id])
REFERENCES [dbo].[MaterialPuerto] ([Id])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.MaterialPuerto_Material_Id]
GO

ALTER TABLE [dbo].[Carga]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Carga_dbo.Vapor_Vapor_Id] FOREIGN KEY([Vapor_Id])
REFERENCES [dbo].[Vapor] ([Id])
GO

ALTER TABLE [dbo].[Carga] CHECK CONSTRAINT [FK_dbo.Carga_dbo.Vapor_Vapor_Id]
GO

