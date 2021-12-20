CREATE TABLE [dbo].[PesoMaximoPorTipoVehiculo] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[TipoVehiculo]          INT            NOT NULL,
	[PesoMaxIngreso]      INT NOT NULL,
	[PesoMaxEgreso]      INT NOT NULL,
	[PesoNetoMaxPlanta] INT NULL, 
    [Activo] BIT NOT NULL, 
    [Centro_Id] INT NOT NULL,
	[PesoNetoMinimo] INT NULL,
    CONSTRAINT [PK_dbo.PesoMaximoPorTipoVehiculo] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.PesoMaximoPorTipoVehiculo_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);