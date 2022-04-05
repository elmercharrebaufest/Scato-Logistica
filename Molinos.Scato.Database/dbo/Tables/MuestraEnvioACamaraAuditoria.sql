CREATE TABLE [dbo].[MuestraEnvioACamaraAuditoria] (
    [Id]                     INT             IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id]           INT             NOT NULL,
    [LoteAuditoria_Id]       INT             NOT NULL,
    [NumeroDocumentoIngreso] NVARCHAR (30)   NULL,
    [ValorCamara]            DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_dbo.MuestraEnvioACamaraAuditoria] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MuestraEnvioACamaraAuditoria_dbo.LoteAuditoria_LoteAuditoria_Id] FOREIGN KEY ([LoteAuditoria_Id]) REFERENCES [dbo].[LoteAuditoria] ([Id]),
    CONSTRAINT [FK_dbo.MuestraEnvioACamaraAuditoria_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);

