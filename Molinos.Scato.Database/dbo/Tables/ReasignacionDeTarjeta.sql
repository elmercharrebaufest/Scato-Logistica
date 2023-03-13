CREATE TABLE [dbo].[ReasignacionDeTarjeta] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [TipoDocumentoIngreso]   INT            NOT NULL,
    [NumeroDocumentoIngreso] NVARCHAR (15)  NOT NULL,
    [NroTarjetaRfidAsignada] NVARCHAR (10)  NOT NULL,
    [NroTarjetaRfidNueva]    NVARCHAR (10)  NOT NULL,
    [UsuarioNombre]          NVARCHAR (20)  NOT NULL,
    [Fecha]                  DATETIME       NOT NULL,
    [Motivo]                 NVARCHAR (20)  NULL,
    [Etapa]                  NVARCHAR (100) NULL,
    [Patente]                NVARCHAR (20)  NULL,
    CONSTRAINT [PK_dbo.ReasignacionDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

