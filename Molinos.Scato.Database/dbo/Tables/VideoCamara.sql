CREATE TABLE [dbo].[VideoCamara] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]                     NVARCHAR (50)  NOT NULL,
    [Descripcion]                NVARCHAR (100) NULL,
    [Directorio]                 NVARCHAR (100) NULL,
    [PuestoDeTrabajo_Id]         INT            NULL,
    [ActividadPorDispositivo_Id] INT            NULL,
    [Posicion] INT NULL, 
    CONSTRAINT [PK_dbo.VideoCamara] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.VideoCamara_dbo.ActividadPorDispositivo_ActividadPorDispositivo_Id] FOREIGN KEY ([ActividadPorDispositivo_Id]) REFERENCES [dbo].[ActividadPorDispositivo] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.VideoCamara_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Id]
    ON [dbo].[VideoCamara]([PuestoDeTrabajo_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_ActividadPorDispositivo_Id]
    ON [dbo].[VideoCamara]([ActividadPorDispositivo_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


