CREATE TABLE [dbo].[BalanzaModificacionModalidad] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Modalidad]                NVARCHAR (10)  NOT NULL,
    [Fecha]                    DATETIME       NOT NULL,
    [Motivo]                   NVARCHAR (MAX) NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (MAX) NOT NULL,
    [Balanza_Id]               INT            NOT NULL,
    CONSTRAINT [PK_dbo.BalanzaModificacionModalidad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.BalanzaModificacionModalidad_dbo.Balanza_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Balanza_Id]
    ON [dbo].[BalanzaModificacionModalidad]([Balanza_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

