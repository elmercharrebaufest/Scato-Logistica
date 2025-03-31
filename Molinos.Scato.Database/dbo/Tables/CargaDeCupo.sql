CREATE TABLE [dbo].[CargaDeCupo] (
    [Id]                         INT           IDENTITY (1, 1) NOT NULL,
    [Numero]                     NVARCHAR (40) NULL,
    [Cupo]                       NVARCHAR (40) NULL,
    [SinCupo]                    BIT           NOT NULL,
    [Recorrido_Id]               INT           NULL,
    [RespuestaSap]               NVARCHAR (40) NULL,
    [PuestoDeTrabajo_Id]         INT           NULL,
    [Material_Id]                INT           NULL,
    [Centro_Id]                  INT           NOT NULL,
    [Fecha]                      DATETIME      NOT NULL,
    [FechaSap]                   DATETIME      NOT NULL,
    [EstuvoPendiente]            BIT           DEFAULT ((0)) NOT NULL,
    [Especial]                   BIT           DEFAULT ((0)) NOT NULL,
    [Camara]                     NVARCHAR (10) NULL,
    [FotoRutaDestino]            VARCHAR (200) NULL,
    [NumeroCartaPorte]           NVARCHAR (40) NULL,
    [CTG]                        NVARCHAR (40) NULL,
    [CodEstab]                   NVARCHAR (40) NULL,
    [RtteComercialCodigoSap]     NVARCHAR (40) NULL,
    [TitularCartaPorteCodigoSap] NVARCHAR (40) NULL,
    [Patente]                    NVARCHAR (40) NULL,
    [FotoCamionRutaDestino]      VARCHAR (200) NULL,
    [Reingresado]                BIT           DEFAULT ((0)) NOT NULL,
    [SacoTurnoConCircular]       BIT           DEFAULT ((0)) NOT NULL,
    [LlegoEnHorario]             BIT           DEFAULT ((0)) NOT NULL,
    [CPE]                        BIT           DEFAULT ((0)) NOT NULL,
    [FotoRutaSustentable]        NVARCHAR(255) NULL,
    [EnProgresoAutomatico]       BIT           DEFAULT((0)) NOT NULL,
    [IngresoAvanceCPEAutomatico] BIT           DEFAULT((0)) NOT NULL,
    [FleteMOA]                   NVARCHAR (7) NULL,
    [TipoOrdenCargaNoGranos] INT NULL,
    [TipoVehiculo] INT DEFAULT ((0)) NULL,
    CONSTRAINT [PK_dbo.CargaDeCupo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CargaDeCupo_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.CargaDeCupo_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.CargaDeCupo_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id]),
    CONSTRAINT [FK_dbo.CargaDeCupo_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [IX_CargaDeCupo_Recorrido_IdCentro_Id]
    ON [dbo].[CargaDeCupo]([Recorrido_Id] ASC, [Centro_Id] ASC, [Numero] ASC, [Cupo] ASC, [Fecha] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


GO 
