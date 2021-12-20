CREATE TABLE [dbo].[LoteDeRedespacho] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [Almacen]		NVARCHAR (50) NULL,
	[Centro]		NVARCHAR (50) NULL,
	[Lote]		NVARCHAR (50) NULL,
	[Material]		NVARCHAR (50) NULL,
	[Stock]		NVARCHAR (50) NULL,

	[Recorrido_Id]         INT NOT NULL,
    CONSTRAINT	[FK_dbo.LoteDeRedespacho_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_dbo.LoteDeRedespacho] PRIMARY KEY CLUSTERED ([Id] ASC)
);
