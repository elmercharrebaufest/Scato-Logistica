CREATE TABLE [dbo].[EstablecimientoProveedor]
(
	[Establecimiento_Id] INT NOT NULL, 
    [Proveedor_Id] INT NOT NULL,
	CONSTRAINT [PK_dbo.EstablecimientoProveedor] PRIMARY KEY CLUSTERED ([Establecimiento_Id] ASC, [Proveedor_Id] ASC),
    CONSTRAINT [FK_dbo.EstablecimientoProveedor_dbo.Establecimiento_Establecimiento_Id] FOREIGN KEY ([Establecimiento_Id]) REFERENCES [dbo].[Establecimiento] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.EstablecimientoProveedor_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]) ON DELETE CASCADE
)
