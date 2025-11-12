CREATE TABLE [dbo].[MonitoreoServicioExterno] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]                NVARCHAR (50)   NOT NULL,
    [HealthCheckUrl]        NVARCHAR (200)  NOT NULL,
    [UltimoEstado]          INT             NOT NULL DEFAULT (0),
    [UltimaVerificacion]    DATETIME        NULL,
	[HealthCheckConfig]     NVARCHAR (MAX)  NULL,
    [KeyJob]                NVARCHAR (100)  NOT NULL,
    CONSTRAINT [PK_dbo.MonitoreoServicioExterno] PRIMARY KEY CLUSTERED ([Id] ASC)
);