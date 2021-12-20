CREATE TABLE [dbo].[ConfiguracionDeTablaCaracteristicaDeCalidad] (
	[Id]               INT            IDENTITY (1, 1) NOT NULL,
    [ConfiguracionDeTabla_Id]      INT NOT NULL,
    [CaracteristicaDeCalidad_Id] INT NOT NULL,
	CONSTRAINT [PK_dbo.ConfiguracionDeTablaCaracteristicaDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ConfiguracionDeTablaCaracteristicaDeCalidad_dbo.ConfiguracionDeTabla_ConfiguracionDeTabla_Id] FOREIGN KEY ([ConfiguracionDeTabla_Id]) REFERENCES [dbo].[ConfiguracionDeTabla] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ConfiguracionDeTablaCaracteristicaDeCalidad_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ConfiguracionDeTabla_Id]
    ON [dbo].[ConfiguracionDeTablaCaracteristicaDeCalidad]([ConfiguracionDeTabla_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CaracteristicaDeCalidad_Id]
    ON [dbo].[ConfiguracionDeTablaCaracteristicaDeCalidad]([CaracteristicaDeCalidad_Id] ASC);

