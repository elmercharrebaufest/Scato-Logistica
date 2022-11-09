CREATE TABLE [dbo].[MuestraEnvioACamara] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [NombreUsuario]        NVARCHAR (20) NOT NULL,
    [Calado_Id]            INT           NOT NULL,
    [Camara_Id]            INT           NOT NULL,
    [CartaPorte_Id]        INT           NULL,
    [PesoNeto]             INT           NULL,
    [FechaDescarga]        DATETIME      NULL,
    [NroMuestra]           VARCHAR (15)  NOT NULL,
    [NroMuestraTerceros]   VARCHAR (14)  NULL,
    [Lote_Id]              INT           NULL,
    [EstadoMuestra]        INT           DEFAULT ((0)) NOT NULL,
    [Patente]              NVARCHAR (20) NULL,
    [TieneAnalisisInterno] BIT           DEFAULT ((0)) NOT NULL,
    [TipoDocumento]        INT           NULL,
    [NroDocumento]         NVARCHAR (40) NULL,
    [Centro_Id]            INT           NULL,
    [GeneroMicroMuestras]  BIT           DEFAULT ((0)) NOT NULL,
    [HuboExcepcion]        BIT           DEFAULT ((0)) NOT NULL,
    [EsPreLote] BIT NULL, 
    CONSTRAINT [PK_dbo.MuestraEnvioACamara] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.Calado_Calado_Id] FOREIGN KEY ([Calado_Id]) REFERENCES [dbo].[Calado] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.Camara_Camara_Id] FOREIGN KEY ([Camara_Id]) REFERENCES [dbo].[Camara] ([Id]),
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.CartaPorte_CartaPorte_Id] FOREIGN KEY ([CartaPorte_Id]) REFERENCES [dbo].[CartaPorte] ([Id]),
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.Lote_Lote_Id] FOREIGN KEY ([Lote_Id]) REFERENCES [dbo].[Lote] ([Id]),
    CONSTRAINT [FK_dbo.MuestraEnvioACamara_dbo.TipoDocumentoIngreso_TipoDocumentoIngreso_Id] FOREIGN KEY ([TipoDocumento]) REFERENCES [dbo].[TipoDocumentoIngreso] ([Id])
);



GO

CREATE NONCLUSTERED INDEX [MuestraEnvioACamara_dbo_Calado_Id]
    ON [dbo].[MuestraEnvioACamara]([Calado_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO