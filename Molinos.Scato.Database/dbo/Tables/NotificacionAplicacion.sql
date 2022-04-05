CREATE TABLE [dbo].[NotificacionAplicacion] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [TipoAccion]  NVARCHAR (25)   NOT NULL,
    [FechaAccion] DATETIME        NOT NULL,
    [Usuario]     NVARCHAR (30)   NOT NULL,
    [Titulo]      NVARCHAR (50)   NOT NULL,
    [Detalle]     NVARCHAR (1000) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON)
);


