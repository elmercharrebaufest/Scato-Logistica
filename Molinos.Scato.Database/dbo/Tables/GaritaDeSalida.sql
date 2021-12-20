CREATE TABLE [dbo].[GaritaDeSalida] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
    [CajaId]      NVARCHAR (40) NULL,
    [CredencialMercadoPago_Id]      int NOT NULL,
    [PuestoDeTrabajo_Id]      int NOT NULL,
    
	CONSTRAINT [FK_dbo.GaritaDeSalida_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]),
	CONSTRAINT [FK_dbo.GaritaDeSalida_dbo.CredencialMercadoPago_CredencialMercadoPago_Id] FOREIGN KEY ([CredencialMercadoPago_Id]) REFERENCES [dbo].[CredencialMercadoPago] ([Id]),
	CONSTRAINT [PK_dbo.GaritaDeSalida] PRIMARY KEY CLUSTERED ([Id] ASC)
)