CREATE TABLE [dbo].[BocaDestino] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[Proveedor_Id]         INT NOT NULL,
    [NombreBocaDeDestino]         NVARCHAR (30) NOT NULL,
    [CodigoPostal]  NVARCHAR (10) NULL,
	[CodigoONCCA]  NVARCHAR (20) NOT NULL,
    [Domicilio]    NVARCHAR (35) NOT NULL,
    [Localidad_Id] INT            NOT NULL,
    [Provincia_Id] INT            NOT NULL,
	[Pais_Id]    INT	 NOT NULL,
    CONSTRAINT [PK_dbo.BocaDestino] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.BocaDestino_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
	CONSTRAINT [FK_dbo.BocaDestino_dbo.Pais_Pais_Id] FOREIGN KEY ([Pais_Id]) REFERENCES [dbo].[Pais] ([Id]),
	CONSTRAINT [FK_dbo.BocaDestino_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    CONSTRAINT [FK_dbo.BocaDestino_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[BocaDestino]([Localidad_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[BocaDestino]([Provincia_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Pais_Id]
    ON [dbo].[BocaDestino]([Pais_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Proveedor_Id]
    ON [dbo].[BocaDestino]([Proveedor_Id] ASC);