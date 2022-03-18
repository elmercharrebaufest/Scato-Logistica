CREATE TABLE [dbo].[PuestoDeTrabajo] (
    [Id]                           INT            IDENTITY (1, 1) NOT NULL,
    [NombrePuesto]                 NVARCHAR (35)  NOT NULL,
    [Lector]                       NVARCHAR (35)  NOT NULL,
    [Entrada]                      NVARCHAR (400) NULL,
    [EntradaSupervisor]            NVARCHAR (400) NOT NULL,
    [SensorQuiebre]                NVARCHAR (35)  NOT NULL,
    [VideoCamara]                  NVARCHAR (50)  NULL,
    [VideoCamaraDirectorio]        NVARCHAR (100) NULL,
    [NombrePc]                     NVARCHAR (30)  NOT NULL,
    [Automatico]                   BIT            NOT NULL,
    [PidePatente]                  BIT            NOT NULL,
    [FotoAlMarcarTarjeta]          BIT            DEFAULT ((0)) NOT NULL,
    [ImprimeTarjetaDeAcceso]       BIT            DEFAULT ((0)) NULL,
    [Centro_Id]                    INT            NOT NULL,
    [EncolaLecturas]               BIT            DEFAULT ((1)) NOT NULL,
    [InvisibleEnListaDeTareas]     BIT            DEFAULT ((0)) NOT NULL,
    [Automatizado]                 BIT            DEFAULT ((0)) NOT NULL,
    [SinAfip]                      BIT            DEFAULT ((0)) NOT NULL,
    [SinCupo]                      BIT            DEFAULT ((0)) NOT NULL,
    [SinFotoCartaPorte]            BIT            DEFAULT ((0)) NOT NULL,
    [ImprimeCartaPorte]            BIT            DEFAULT ((0)) NOT NULL,
    [LectorQr]                     NVARCHAR (35)  NULL,
    [CartelLed]                    NVARCHAR (35)  NULL,
    [Balanza_Id]                   INT            NULL,
    [AutomatizadoFull]             BIT            DEFAULT ((0)) NOT NULL,
    [NoAsignaCalleEnGaritaEntrada] BIT            DEFAULT ((1)) NOT NULL,
    [PausaAutoFull]                BIT            DEFAULT ((0)) NOT NULL,
    [Firmware]                     NVARCHAR (300) NULL,
    [CierreSupervisor]             NVARCHAR (400) NULL,
    [CierreEntrada]                NVARCHAR (400) NULL,
    [Concentrador]                 NVARCHAR (100) NULL,
    [OrdenBalanza]                 INT            NULL,
    [IntercomunicadorCodigo]       NVARCHAR (50)  NULL,
    [ActivarRegistroInactividad]   BIT            DEFAULT ((0)) NOT NULL,
    [SemaforoRojo]                 NVARCHAR (100) NULL,
    [SemaforoAmarillo]             NVARCHAR (100) NULL,
    [SemaforoVerde]                NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.PuestoDeTrabajo] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.Balanza_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.PuestoDeTrabajo_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[PuestoDeTrabajo]([Centro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Lector]
    ON [dbo].[PuestoDeTrabajo]([Lector] ASC) WHERE ([Lector] IS NOT NULL) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_NombrePc]
    ON [dbo].[PuestoDeTrabajo]([NombrePc] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_PuestoDeTrabajo_Balanza]
    ON [dbo].[PuestoDeTrabajo]([Balanza_Id] ASC) WHERE ([Balanza_Id] IS NOT NULL) WITH (STATISTICS_NORECOMPUTE = ON);

