CREATE TABLE [dbo].[CaladoEnPlantaPorCaracteristica] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [CaracteristicaDeCalidad_Id]      INT             NOT NULL,
    [CaladoEnPlanta_Id]                       INT             NOT NULL,
    [ValorCalado]                     DECIMAL (18, 2) NULL,
	[ValorCaladoEnPlanta]                     DECIMAL (18, 2) NULL,
    [Unidad]                          NVARCHAR (20)   NOT NULL,
    [Rango]                           NVARCHAR(20) NULL,
    CONSTRAINT [PK_dbo.CaladoEnPlantaPorCaracteristica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CaladoEnPlantaPorCaracteristica_dbo.CaladoEnPlanta_CaladoEnPlanta_Id] FOREIGN KEY ([CaladoEnPlanta_Id]) REFERENCES [dbo].[CaladoEnPlanta] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CaladoEnPlantaPorCaracteristica_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CaladoEnPlanta_Id]
    ON [dbo].[CaladoEnPlantaPorCaracteristica]([CaladoEnPlanta_Id] ASC)
	INCLUDE(CaracteristicaDeCalidad_Id,Id);

GO
CREATE NONCLUSTERED INDEX [IX_CaracteristicaDeCalidad_Id]
    ON [dbo].[CaladoEnPlantaPorCaracteristica]([CaracteristicaDeCalidad_Id] ASC);
GO

