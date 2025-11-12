CREATE TABLE [dbo].[RegistroJobEjecucion] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [NombreProceso]     NVARCHAR (50)   NOT NULL,
	[Descripcion]	    NVARCHAR (255)  NULL,
	[FechaEjecucion]    DATETIME NOT NULL
    CONSTRAINT [PK_dbo.RegistroJobEjecucion] PRIMARY KEY CLUSTERED ([Id] ASC),
);
