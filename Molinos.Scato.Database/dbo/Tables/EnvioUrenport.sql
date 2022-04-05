CREATE TABLE [dbo].[EnvioUrenport] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [NumeroDocumentoIngreso] NVARCHAR (15)  NOT NULL,
    [TipoDoc]                NVARCHAR (10)  NOT NULL,
    [Ruta]                   NVARCHAR (200) NULL,
    [Recorrido_Id]           INT            NULL,
    [Estado]                 INT            NOT NULL,
    [Fecha]                  DATETIME       NOT NULL,
    [Error]                  NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.EnvioUrenport] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.EnvioUrenport_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);



GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[EnvioUrenport]([Recorrido_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);

