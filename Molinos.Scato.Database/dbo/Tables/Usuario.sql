CREATE TABLE [dbo].[Usuario] (
    [Id]                                       INT             IDENTITY (1, 1) NOT NULL,
    [NombreUsuario]                            NVARCHAR (40)   NOT NULL,
    [Apellido]                                 NVARCHAR (40)   NOT NULL,
    [Nombre]                                   NVARCHAR (40)   NOT NULL,
    [Email]                                    NVARCHAR (80)   NULL,
    [UltimoLogin]                              DATETIME        NULL,
    [Matricula]                                NVARCHAR (40)   NULL,
    [Firma]                                    VARCHAR (8000)  NULL,
    [AvisoQuiebreApertura]                     BIT             DEFAULT ((0)) NOT NULL,
    [AvisoQuiebreCierre]                       BIT             DEFAULT ((0)) NOT NULL,
    [ReasignacionDeTarjeta]                    BIT             DEFAULT ((0)) NOT NULL,
    [AvisoAutorizarTiempoEnTransito]           BIT             DEFAULT ((0)) NOT NULL,
    [FirmaImagen]                              VARBINARY (MAX) NULL,
    [AvisoAutorizarTiempoEnTransitoConfirmado] BIT             DEFAULT ((0)) NOT NULL,
    [AvisoAutorizarTiempoEnTransitoRechazado]  BIT             DEFAULT ((0)) NOT NULL,
    [AvisoContingencia]                        BIT             DEFAULT ((0)) NOT NULL,
    [AvisoEntregaHexano]                       BIT             DEFAULT ((0)) NOT NULL,
    [AvisoLineUp]                              BIT             DEFAULT ((0)) NOT NULL,
    [AvisoCambioPinchazosPorCalada]            BIT             DEFAULT ((0)) NOT NULL,
    [AvisoPlanoDeCarga]                        BIT             DEFAULT ((0)) NOT NULL,
    [AvisoModuloDeCarga]                       BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Usuario] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON)
);



GO
