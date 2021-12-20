CREATE TABLE [dbo].[GraficoDePlanta] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [NombreActividad]      NVARCHAR (200) NOT NULL,
    [CantidadCamionesNoDemorados] INT NULL, 
    [CantidadCamionesDemorados] INT NULL, 
    [Color] NVARCHAR(100) NULL, 
    [Rango] INT NULL, 
    [Centro_Id] INT NOT NULL, 
	[Sector] INT NULL,
    CONSTRAINT [PK_dbo.GraficoDePlanta] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.GraficoDePlanta_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])

);

