CREATE TABLE [dbo].[HumedimetroModificacionModalidad] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Modalidad]                NVARCHAR (10)  NOT NULL,
    [Fecha]                    DATETIME       NOT NULL,
    [Motivo]                   NVARCHAR (500) NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (MAX) NOT NULL,
    [Humedimetro_Id]           INT            NOT NULL,
    CONSTRAINT [PK_dbo.HumedimetroModificacionModalidad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.HumedimetroModificacionModalidad_dbo.Humedimetro_Humedimetro_Id] FOREIGN KEY ([Humedimetro_Id]) REFERENCES [dbo].[Humedimetro] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Humedimetro_Id]
    ON [dbo].[HumedimetroModificacionModalidad]([Humedimetro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

