CREATE TABLE [dbo].[Stock] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Material_Id]  INT             DEFAULT ((1)) NOT NULL,
    [Proveedor_Id] INT             NOT NULL,
    [Fecha]        DATETIME        NOT NULL,
    [Cantidad]     DECIMAL (18, 2) NOT NULL,
    [Recorrido_Id] INT             NULL,
    CONSTRAINT [PK_dbo.Stock] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Stock_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.Stock_dbo.Proveedor_Proveedor_Id] FOREIGN KEY ([Proveedor_Id]) REFERENCES [dbo].[Proveedor] ([Id]),
    CONSTRAINT [FK_dbo.Stock_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);

