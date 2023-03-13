CREATE TABLE [dbo].[ImpEtiquetaRubrosAnalizar] (
    [Id]                    INT            NOT NULL,
    [Centro]                NVARCHAR (50)  NULL,
    [NumeroCartaPorte]      NVARCHAR (50)  NULL,
    [PatenteCamion]         NVARCHAR (50)  NULL,
    [AnalisisSeleccionados] NVARCHAR (500) NULL,
    CONSTRAINT [PK_ImpEtiquetaRubrosAnalizar] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

