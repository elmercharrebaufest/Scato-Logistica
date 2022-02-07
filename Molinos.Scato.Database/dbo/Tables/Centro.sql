CREATE TABLE [dbo].[Centro] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
	[CodigoSAP]                  NVARCHAR (20)			 NOT NULL,
    [Descripcion]                NVARCHAR (40)  NULL,
    [MailBarreras]               NVARCHAR (45)  NULL,
    [CantEtiquetasMicromuestras] INT            NULL,
    [CodigoDeChamico]            INT            NULL,
    [CodigoDeInsectos]           INT            NULL,
    [CodigoDeTenorAzucarino]     INT            NULL,
    [Sociedad]                   NVARCHAR (40)  NULL,
    [TiempoMaxEntreActividades]  INT            NULL,
    [CasillerosPorHumedad]       BIT            NOT NULL,
    [UsaConfirmacionDescarga]    BIT            NOT NULL,
    [ReingresaPatenteAlPesar]    BIT            NOT NULL,
    [ReingresaPatenteEnCalado]   BIT            NOT NULL,
    [UsaBinPallet]               BIT            NOT NULL,
    [UsaNumeroMuestraTerceros]   BIT            NOT NULL,
    [ImprimeReciboMunicipal]     BIT            NOT NULL,
    [SolicitaConfirmarCTG]       BIT            NOT NULL,
	[LibroOnccaPorDescripcionCorta]       BIT            NOT NULL DEFAULT 0,
    [NumeroLoteInicial]          INT            NULL,
    [Cuit]                       NVARCHAR (13)  NULL,
    [NumeroCAI]                  INT            NULL,
    [VigenciaDesde]              DATETIME       NULL,
    [VigenciaHasta]              DATETIME       NULL,
    [MailVencimientoCAI]         NVARCHAR (45)  NULL,
    [CamaraDefault_Id]                  INT            NULL,
    [Provincia_Id]               INT            NULL,
    [Localidad_Id]               INT            NULL,
	[Direccion]         NVARCHAR (100)  NULL,
	[EsVirtual]                    BIT            NOT NULL DEFAULT 0,
	[CodigoEstablecimiento]       NVARCHAR (45)  NULL,
	[CodigoPostal]                NVARCHAR (8)  NULL,
	[NumeroOrigenCamaraBsAs]      NVARCHAR (4)  NULL,
	[AsignaBalanzaEnComando]       BIT            NOT NULL default 1,

	[RazonSocial]  NVARCHAR (35) NULL,
	[NumeroINV] NVARCHAR(10) NULL, 
	[IngresosBrutos] NVARCHAR(30) NULL, 
    [RequiereCupo] BIT NOT NULL DEFAULT 0, 
	[ValidarCupo] BIT NOT NULL DEFAULT 0, 
    [ModificaAlmacenEnPesada] BIT NOT NULL DEFAULT 0, 

	[CodigoDeAduana]                       NVARCHAR (13)  NULL,
	[EncolaBajaCtgAutomatico] BIT NOT NULL DEFAULT 0,
	[DescargaCartaPortePorCtg] BIT NOT NULL DEFAULT 0,
	[CodigoSAPEspecial]                  NVARCHAR (40)			 NULL,
    [HorarioDesde]                  INT            NULL ,
    [HorarioHasta]                  INT            NULL ,
	[CodigoEstacionMeteorologica] NVARCHAR (40) NULL ,
    [ValidarLimiteDeCreditoVentaEnSAP] BIT NOT NULL DEFAULT 1, 
    [ValidarLimiteMinimoDePeso] BIT NOT NULL DEFAULT 0, 
    [LimiteMinimoDePeso] INT NULL, 
	[TomarFotoEnMesa] BIT NOT NULL DEFAULT 0, 
	[LeerCPDeFoto] BIT NOT NULL DEFAULT 0,
	[ToleranciaPatenteLeida] INT            NULL,
	[ModificaPinchazosPorCalada] BIT NOT NULL DEFAULT 0,
    [InformaCircular] BIT NOT NULL DEFAULT 0,
    [MinutosEsperaCircular] INT NULL,
    [NotificarCamioneroCircular] BIT NOT NULL DEFAULT 0,
    [Sucursal]  INT NULL,
    [Planta]  INT NULL,
	[ContingenciaAfipCpe]          BIT            NOT NULL default 0,
    [MinutosInactividadCalado] INT NULL,
    [LimiteCamionesCalado] INT NULL DEFAULT 8, 
    CONSTRAINT [PK_dbo.Centro] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Centro_dbo.Camara_Camara_Id] FOREIGN KEY ([CamaraDefault_Id]) REFERENCES [dbo].[Camara] ([Id]),
    CONSTRAINT [FK_dbo.Centro_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Centro_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id]),
	CONSTRAINT [UK_Centro_CodigoSAP] UNIQUE (CodigoSAP)
);


GO
CREATE NONCLUSTERED INDEX [IX_Camara_Id]
    ON [dbo].[Centro]([CamaraDefault_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Centro]([Provincia_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Centro]([Localidad_Id] ASC);

