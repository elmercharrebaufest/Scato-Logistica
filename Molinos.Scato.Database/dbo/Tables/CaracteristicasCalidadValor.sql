CREATE TABLE [dbo].[CaracteristicasCalidadValor] (
    [Id]                             INT           IDENTITY (1, 1) NOT NULL,
    [Key]                            NVARCHAR (50) NOT NULL,
    [Data]                           NVARCHAR (50) NOT NULL,
    [ImpConstanciaDeEntregaLaser_Id] INT           NOT NULL,
    [Descuento]                      NVARCHAR (30) NULL,
    [EnvioCamara]                    NVARCHAR (30) NULL,
    CONSTRAINT [PK_dbo.CaracteristicasCalidadValor] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CaracteristicasCalidadValor_dbo.ImpConstanciaDeEntregaLaser_ImpConstanciaDeEntregaLaser_Id] FOREIGN KEY ([ImpConstanciaDeEntregaLaser_Id]) REFERENCES [dbo].[ImpConstanciaDeEntregaLaser] ([Id]) ON DELETE CASCADE
);

