CREATE TABLE [dbo].[Permiso] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion]       NVARCHAR (50) NOT NULL,
    [TipoPermiso]       INT           NOT NULL,
    [Codigo]            INT           NOT NULL,
    [ActividadWorkflow] NVARCHAR (50) NULL,
    [NombreActividad]   NVARCHAR (50) NULL,
    CONSTRAINT [PK_dbo.Permiso] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);



GO
