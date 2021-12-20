CREATE TABLE [dbo].[RomaneoItemPedido] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [Romaneo_Id]		INT NOT NULL,
    CONSTRAINT	[FK_dbo.RomaneoItemPedido_dbo.Romaneo_Romaneo_Id] FOREIGN KEY ([Romaneo_Id]) REFERENCES [dbo].[Romaneo] ([Id]) ON DELETE CASCADE,
    [Material_Id]		INT NOT NULL,
	CONSTRAINT	[FK_dbo.RomaneoItemPedido_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    [Ebelp]		NVARCHAR (20) NULL,
	[CantPedido]		NVARCHAR (20) NULL,
	[FecEntrega]		NVARCHAR (20) NULL,
	[PorcentajeExc]		NVARCHAR (20) NULL,
	[Werks]		NVARCHAR (20) NULL,
	CONSTRAINT [PK_dbo.RomaneoItemPedido] PRIMARY KEY CLUSTERED ([Id] ASC)
);
