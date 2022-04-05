CREATE TABLE [dbo].[AutorizarTiempoEnTransito] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]  UNIQUEIDENTIFIER NOT NULL,
    [Mensaje]             NVARCHAR (100)   NULL,
    [Comentario]          NVARCHAR (MAX)   NULL,
    [NombreUsuario]       NVARCHAR (20)    NULL,
    [Actividad]           NVARCHAR (50)    NULL,
    [Decision]            BIT              NOT NULL,
    [Fecha]               DATETIME         NULL,
    [DocumentoIngreso]    NVARCHAR (50)    NULL,
    [NroDocumentoIngreso] NVARCHAR (50)    NULL,
    [Patente]             NVARCHAR (10)    NULL,
    [Material]            NVARCHAR (50)    NULL,
    [Actividad1]          NVARCHAR (50)    NULL,
    [Actividad2]          NVARCHAR (50)    NULL,
    [TiempoAceptado]      NVARCHAR (50)    NULL,
    [TiempoEnTransito]    NVARCHAR (50)    NULL,
    [FechaActividad1]     NVARCHAR (50)    NULL,
    [FechaActividad2]     NVARCHAR (50)    NULL,
    [DiferenciaTiempo]    NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.AutorizarTiempoEnTransito] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

