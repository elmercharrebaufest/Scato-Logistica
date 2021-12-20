CREATE TABLE [dbo].[Cosecha]
(
	[Id]				INT IDENTITY (1, 1) NOT NULL, 
	[Descripcion]           NVARCHAR (5) NOT NULL,
	[EpaPesoDescontado]			BIT DEFAULT 0 NOT NULL,
	CONSTRAINT [PK_dbo.Cosecha] PRIMARY KEY CLUSTERED ([Id] ASC)
)
