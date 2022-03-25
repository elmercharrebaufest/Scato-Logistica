CREATE TABLE [dbo].[TipoDeActividad] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]      NCHAR (5)      NULL,
    [Descripcion] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

