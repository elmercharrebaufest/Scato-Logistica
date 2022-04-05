CREATE TABLE [dbo].[EstadoMaterial] (
    [Id]                          INT            IDENTITY (1, 1) NOT NULL,
    [CamionesEnElDia]             INT            NOT NULL,
    [CamionesEnPlanta]            INT            NOT NULL,
    [TotalIngresosEnElDia]        INT            NOT NULL,
    [Rechazados]                  INT            NOT NULL,
    [Peso]                        INT            NOT NULL,
    [Material]                    NVARCHAR (100) NOT NULL,
    [CentroId]                    INT            NOT NULL,
    [EsGrano]                     BIT            NOT NULL,
    [EsIngreso]                   BIT            NOT NULL,
    [VagonesEnElDia]              INT            NULL,
    [VagonesEnPlanta]             INT            NULL,
    [TotalIngresosVagonesEnElDia] INT            NULL,
    [VagonesRechazados]           INT            NULL,
    CONSTRAINT [PK_dbo.EstadoMaterial] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON)
);

