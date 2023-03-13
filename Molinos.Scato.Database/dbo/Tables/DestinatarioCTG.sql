CREATE TABLE [dbo].[DestinatarioCTG] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [CuitDestinatario] NVARCHAR (50) NULL,
    [Recorrido_Id]     INT           NULL,
    [CanjeRemito]      NVARCHAR (50) NULL,
    [NumeroCCPP]       NVARCHAR (50) NULL,
    [Cosecha]          NVARCHAR (50) NULL,
    [CTG]              NVARCHAR (50) NULL,
    [CuitCanjeador]    NVARCHAR (50) NULL,
    [CuitDestino]      NVARCHAR (50) NULL,
    [Especie]          NVARCHAR (50) NULL,
    [Establecimiento]  NVARCHAR (50) NULL,
    [Estado]           NVARCHAR (50) NULL,
    [FechaConf]        NVARCHAR (50) NULL,
    [PesoNetoCarga]    DECIMAL (18)  NULL,
    [Solicitante]      NVARCHAR (50) NULL,
    [Cupo]             NVARCHAR (16) NULL,
    CONSTRAINT [PK_dbo.DestinatarioCTG] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.DestinatarioCTG_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);



GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[DestinatarioCTG]([Recorrido_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

