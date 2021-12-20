CREATE TABLE [dbo].[RecepcionYRedespacho] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[Caracteristica]      NVARCHAR (100) NULL,
	[Desckilos]      NVARCHAR (100) NULL,
	[Descporc]      NVARCHAR (100) NULL,
	[EntradaOSalida]      NVARCHAR (100) NULL,
	[NumCarPor]      NVARCHAR (100) NULL,
	[Secuencia]      NVARCHAR (100) NULL,
	[TipoMuest]      NVARCHAR (100) NULL,
	[Resultado]      NVARCHAR (100) NULL,
	[IngresosPorCompraDeGranosTransmisionASap_Id] int NOT NULL,

    CONSTRAINT [PK_dbo.RecepcionYRedespacho] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.RecepcionYRedespacho_dbo.IngresosPorCompraDeGranosTransmisionASap_IngresosPorCompraDeGranosTransmisionASap_Id] FOREIGN KEY ([IngresosPorCompraDeGranosTransmisionASap_Id]) REFERENCES [dbo].[IngresosPorCompraDeGranosTransmisionASap] ([Id]) ON DELETE CASCADE
);
