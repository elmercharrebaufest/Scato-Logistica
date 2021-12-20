CREATE TABLE [dbo].[EstadoPuerto] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[FechaCalado]       DATETIME NULL,
	[FechaUbicacion]       DATETIME NULL,
	[FechaAlturaRio]       DATETIME NULL,
	[Calado]       nvarchar(20) NULL,
	[Ubicacion]       nvarchar(20) NULL,
	[AlturaDelRio]       nvarchar(20) NULL,
    CONSTRAINT [PK_dbo.EstadoPuerto] PRIMARY KEY CLUSTERED ([Id] ASC)
    );