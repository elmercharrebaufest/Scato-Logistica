CREATE TABLE [dbo].[UbicacionDeBuquePuerto] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NULL,    
    [Orden] int not NULL,
    CONSTRAINT [PK_dbo.UbicacionDeBuquePuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_UbicacionDeBuquePuerto_Nombre] UNIQUE (Nombre),
    CONSTRAINT [UK_UbicacionDeBuquePuerto_Orden] UNIQUE (Orden)
);
GO