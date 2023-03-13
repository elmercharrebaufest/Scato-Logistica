CREATE TABLE [dbo].[DocumentoBorrado] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [TipoDocumentoIngreso]   INT            NOT NULL,
    [NumeroDocumentoIngreso] NVARCHAR (40)  NULL,
    [Patente]                NVARCHAR (60)  NULL,
    [Fecha]                  DATETIME       NULL,
    [Motivo]                 NVARCHAR (255) NULL,
    [NombreUsuario]          NVARCHAR (20)  NULL,
    [Centro_Id]              INT            NULL,
    [UltimaActividad]        NVARCHAR (100) NULL,
    [EtapaDeBorrado]         NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.DocumentoBorrado] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.DocumentoBorrado_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[DocumentoBorrado]([Centro_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

