CREATE TABLE [dbo].[DensidadPorTemperaturaDeMaterial]
(
	[Id] INT NOT NULL  IDENTITY, 
    [MaterialPuerto_Id] INT NOT NULL, 
    [Grado] INT NULL, 
    [Densidad] DECIMAL(5,4) NULL, 
    PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_dboDensidadPorTemperaturaDeMaterial_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
);
