CREATE TABLE [dbo].[ExcepcionEnvioCamara] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Material_Id]			INT NOT NULL,
    [CaracteristicaMaterial_Id] INT NOT NULL,
    [TipoComercial_Id] INT NOT NULL, 
    [Proveedor_Id] INT NOT NULL, 
    [Entregador_Id] INT NOT NULL, 
    CONSTRAINT [PK_dbo.ExcepcionEnvioCamara] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ExcepcionEnvioCamara_dbo.Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionEnvioCamara_dbo.Caracteristica_Id] FOREIGN KEY ([CaracteristicaMaterial_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionEnvioCamara_dbo.TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionEnvioCamara_dbo.Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionEnvioCamara_dbo.Entregador_Id] FOREIGN KEY ([Entregador_Id]) REFERENCES [dbo].[Entregador] ([Id])
);

