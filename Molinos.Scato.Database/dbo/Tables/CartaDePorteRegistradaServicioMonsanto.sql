CREATE TABLE [dbo].[CartaDePorteRegistradaServicioMonsanto] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,

	TipoAnalisis NVARCHAR (100) NULL,
	LaboratorioRazonSocial NVARCHAR (100) NULL,
	LaboratorioCuit NVARCHAR (100) NULL,
	Recorrido_Id Int NOT NULL,

	CONSTRAINT [PK_dbo.CartaDePorteRegistradaServicioMonsanto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CartaDePorteRegistradaServicioMonsanto_dbo.Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);
GO
