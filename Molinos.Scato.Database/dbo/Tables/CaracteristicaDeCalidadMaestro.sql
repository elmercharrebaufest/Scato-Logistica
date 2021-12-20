CREATE TABLE [dbo].[CaracteristicaDeCalidadMaestro]
(
    [Id]	INT	IDENTITY (1, 1) NOT NULL,
	[Descripcion]        NVARCHAR (30)  NOT NULL
	CONSTRAINT [PK_dbo.CaracteristicaDeCalidadMaestro] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO