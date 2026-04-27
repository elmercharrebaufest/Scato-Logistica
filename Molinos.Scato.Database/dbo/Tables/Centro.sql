CREATE TABLE [dbo].[Centro] (
    [Id]                               INT            IDENTITY (1, 1) NOT NULL,
    [CodigoSAP]                        NVARCHAR (20)  NOT NULL,
    [Descripcion]                      NVARCHAR (40)  NULL,
    [MailBarreras]                     NVARCHAR (45)  NULL,
    [CantEtiquetasMicromuestras]       INT            NULL,
    [CodigoDeChamico]                  INT            NULL,
    [CodigoDeInsectos]                 INT            NULL,
    [CodigoDeTenorAzucarino]           INT            NULL,
    [Sociedad]                         NVARCHAR (40)  NULL,
    [TiempoMaxEntreActividades]        INT            NULL,
    [CasillerosPorHumedad]             BIT            NOT NULL,
    [UsaConfirmacionDescarga]          BIT            NOT NULL,
    [ReingresaPatenteAlPesar]          BIT            NOT NULL,
    [ReingresaPatenteEnCalado]         BIT            NOT NULL,
    [UsaBinPallet]                     BIT            NOT NULL,
    [UsaNumeroMuestraTerceros]         BIT            NOT NULL,
    [ImprimeReciboMunicipal]           BIT            NOT NULL,
    [SolicitaConfirmarCTG]             BIT            NOT NULL,
    [LibroOnccaPorDescripcionCorta]    BIT            DEFAULT ((0)) NOT NULL,
    [NumeroLoteInicial]                INT            NULL,
    [Cuit]                             NVARCHAR (13)  NULL,
    [NumeroCAI]                        INT            NULL,
    [VigenciaDesde]                    DATETIME       NULL,
    [VigenciaHasta]                    DATETIME       NULL,
    [MailVencimientoCAI]               NVARCHAR (45)  NULL,
    [CamaraDefault_Id]                 INT            NULL,
    [Provincia_Id]                     INT            NULL,
    [Localidad_Id]                     INT            NULL,
    [Direccion]                        NVARCHAR (100) NULL,
    [EsVirtual]                        BIT            DEFAULT ((0)) NOT NULL,
    [CodigoEstablecimiento]            NVARCHAR (45)  NULL,
    [CodigoPostal]                     NVARCHAR (8)   NULL,
    [NumeroOrigenCamaraBsAs]           NVARCHAR (4)   NULL,
    [AsignaBalanzaEnComando]           BIT            DEFAULT ((1)) NOT NULL,
    [RazonSocial]                      NVARCHAR (35)  NULL,
    [NumeroINV]                        NVARCHAR (10)  NULL,
    [IngresosBrutos]                   NVARCHAR (30)  NULL,
    [RequiereCupo]                     BIT            DEFAULT ((0)) NOT NULL,
    [ValidarCupo]                      BIT            DEFAULT ((0)) NOT NULL,
    [ModificaAlmacenEnPesada]          BIT            DEFAULT ((0)) NOT NULL,
    [CodigoDeAduana]                   NVARCHAR (13)  NULL,
    [EncolaBajaCtgAutomatico]          BIT            DEFAULT ((0)) NOT NULL,
    [DescargaCartaPortePorCtg]         BIT            DEFAULT ((0)) NOT NULL,
    [CodigoSAPEspecial]                NVARCHAR (40)  NULL,
    [HorarioDesde]                     INT            NULL,
    [HorarioHasta]                     INT            NULL,
    [CodigoEstacionMeteorologica]      NVARCHAR (40)  NULL,
    [ValidarLimiteDeCreditoVentaEnSAP] BIT            DEFAULT ((1)) NOT NULL,
    [ValidarLimiteMinimoDePeso]        BIT            DEFAULT ((0)) NOT NULL,
    [LimiteMinimoDePeso]               INT            NULL,
    [TomarFotoEnMesa]                  BIT            DEFAULT ((0)) NOT NULL,
    [LeerCPDeFoto]                     BIT            DEFAULT ((0)) NOT NULL,
    [ToleranciaPatenteLeida]           INT            NULL,
    [ModificaPinchazosPorCalada]       BIT            DEFAULT ((0)) NOT NULL,
    [InformaCircular]                  BIT            DEFAULT ((0)) NOT NULL,
    [MinutosEsperaCircular]            INT            NULL,
    [NotificarCamioneroCircular]       BIT            DEFAULT ((0)) NOT NULL,
    [Sucursal]                         INT            NULL,
    [Planta]                           INT            NULL,
    [MinutosInactividadCalado]         INT            NULL,
    [LimiteCamionesCalado]             INT            DEFAULT ((8)) NULL,
    [FechaEjecucionCacheoCPE]          DATETIME       NULL,
    [ErrorCacheoAfipCPE]               NVARCHAR (MAX) NULL,
    [FotosPath]                        VARCHAR (200)  NULL,
    [AvanzaCpe] BIT NOT NULL DEFAULT 0, 
    [MinutosEsperaPrecalado] INT DEFAULT ((0)) NULL,
    [Domicilio_Id] INT NULL, 
    [ValidarAutomaticamenteCTGDG] BIT DEFAULT ((0)) NOT NULL, 
    [PlantaDG] INT NULL, 
    CONSTRAINT [PK_dbo.Centro] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Centro_dbo.Camara_Camara_Id] FOREIGN KEY ([CamaraDefault_Id]) REFERENCES [dbo].[Camara] ([Id]),
    CONSTRAINT [FK_dbo.Centro_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Centro_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id]),
    CONSTRAINT [FK_dbo.Centro_dbo.Domicilio_Domicilio_Id] FOREIGN KEY ([Domicilio_Id]) REFERENCES [dbo].[Domicilio] ([Id]),
    CONSTRAINT [UK_Centro_CodigoSAP] UNIQUE NONCLUSTERED ([CodigoSAP] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Camara_Id]
    ON [dbo].[Centro]([CamaraDefault_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Centro]([Provincia_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Centro]([Localidad_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



