CREATE TABLE [dbo].[Notificacion] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [Grupo]      NVARCHAR (50)   NOT NULL,
    [TipoAlerta] INT             NOT NULL,
    [Mensaje]    NVARCHAR (1000) NOT NULL,
    [Hora]       DATETIME        NOT NULL,
    [Leido]      BIT             NOT NULL,
    [PuestoId]   INT             NULL,
    CONSTRAINT [PK_dbo.Notificacion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);




GO
CREATE NONCLUSTERED INDEX [IX_Leido_Grupo]
    ON [dbo].[Notificacion]([Leido] ASC, [Grupo] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_Grupo_TipoAlerta_Leido_incl_varios]
    ON [dbo].[Notificacion]([Grupo] ASC, [TipoAlerta] ASC, [Leido] ASC)
    INCLUDE([Mensaje], [Hora], [PuestoId]) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_Grupo_incl_varios]
    ON [dbo].[Notificacion]([Grupo] ASC)
    INCLUDE([TipoAlerta], [Mensaje], [Hora], [Leido], [PuestoId]) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

