CREATE TABLE [dbo].[ConfiguracionAutomatizacionEtapas] (
    [Id]                            INT IDENTITY (1, 1) NOT NULL,
    [Descripcion]                   NVARCHAR (100) NOT NULL,
    [CentroId]                      INT NOT NULL,
    [Actividad]                     NVARCHAR (55) NOT NULL,
    [MinutosEjecucion]              INT NOT NULL default 0,
    [FechaCreacion]					DATETIME NOT NULL,
    [Deshabilitada]                 BIT NOT NULL default 0,

    CONSTRAINT [PK_dbo.ConfiguracionAutomatizacionEtapas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ConfiguracionAutomatizacionEtapas_dbo.ConfiguracionAutomatizacionEtapas_Centro_Id] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [UNIQUE_dbo.ConfiguracionAutomatizacionEtapas_CentroId_Actividad] UNIQUE ([CentroId], [Actividad])
);