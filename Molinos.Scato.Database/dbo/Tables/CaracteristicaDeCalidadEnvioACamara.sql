CREATE TABLE [dbo].[CaracteristicaDeCalidadMuestraEnvioACamara] (
    [MuestraEnvioACamara_Id]      INT NOT NULL,
    [CaracteristicaDeCalidad_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.CaracteristicaDeCalidadMuestraEnvioACamara] PRIMARY KEY CLUSTERED ([MuestraEnvioACamara_Id] ASC, [CaracteristicaDeCalidad_Id] ASC),
    CONSTRAINT [FK_dbo.CaracteristicaDeCalidadMuestraEnvioACamara_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CaracteristicaDeCalidadMuestraEnvioACamara_dbo.EnvioACamara_MuestraEnvioACamara_Id] FOREIGN KEY ([MuestraEnvioACamara_Id]) REFERENCES [dbo].[MuestraEnvioACamara] ([Id]) ON DELETE CASCADE
);
