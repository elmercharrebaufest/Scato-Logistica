CREATE TABLE [dbo].[Establecimiento] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[Proveedor_Id]         INT NOT NULL,
    [NombreDeEstablecimiento]         NVARCHAR (30) NOT NULL,
	[CodigoDeEstablecimiento]  NVARCHAR (10) NULL,
    [CodigoPostal]  NVARCHAR (10) NULL,
    [Domicilio]    NVARCHAR (35) NULL,
    [Localidad_Id] INT            NULL,
    [Provincia_Id] INT            NULL,
	[Anulado] BIT            NOT NULL default 0,
    CONSTRAINT [PK_dbo.Establecimiento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Establecimiento_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
	CONSTRAINT [FK_dbo.Establecimiento_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    CONSTRAINT [FK_dbo.Establecimiento_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Establecimiento]([Localidad_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Establecimiento]([Provincia_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Proveedor_Id]
    ON [dbo].[Establecimiento]([Proveedor_Id] ASC);