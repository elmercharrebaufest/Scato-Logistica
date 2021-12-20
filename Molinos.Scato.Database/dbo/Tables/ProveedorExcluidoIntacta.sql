CREATE TABLE [dbo].[ProveedorExcluidoIntacta] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [Proveedor_Id]      INT             NOT NULL,
    CONSTRAINT [PK_dbo.ProveedorExcluidoIntacta] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ProveedorExcluidoIntacta_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
);

GO
CREATE NONCLUSTERED INDEX [IX_Proveedor_Id]
    ON [dbo].[ProveedorExcluidoIntacta]([Proveedor_Id] ASC);
