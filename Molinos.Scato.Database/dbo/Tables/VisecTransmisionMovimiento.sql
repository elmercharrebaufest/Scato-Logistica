CREATE TABLE [dbo].[VisecTransmisionMovimiento] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [VisecTransmision_Id]       INT            NOT NULL,
    [NumeroRENSPA]              NVARCHAR (17)  NULL,
    [NumeroCTGAsignado]         NVARCHAR (12)  NULL,
    [PesoNetoCargaKgPorUP]      INT            NULL,
    [PesoNetoDescargaKgPorUP]   INT            NULL,
    [PesoIngresoStockKg]        INT            NULL,
    [UltimoAlmacenamiento]      NVARCHAR (MAX) NULL,
    [TipoMovimiento]            INT            NOT NULL,
    CONSTRAINT [PK_dbo.VisecTransmisionMovimiento] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
	CONSTRAINT [FK_dbo.VisecTransmisionMovimiento_dbo.VisecTransmision_Id] FOREIGN KEY ([VisecTransmision_Id]) REFERENCES [dbo].[VisecTransmision] ([Id]) ON DELETE CASCADE
);