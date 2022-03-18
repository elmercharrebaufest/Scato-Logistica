CREATE TABLE [dbo].[Transportista] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Cuit]         NVARCHAR (13) NOT NULL,
    [RazonSocial]  NVARCHAR (50) NOT NULL,
    [Domicilio]    NVARCHAR (35) DEFAULT ('Sin Direccion') NULL,
    [Localidad_Id] INT           NULL,
    [Provincia_Id] INT           NULL,
    [MedioDePago]  INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Transportista] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Transportista_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
    CONSTRAINT [FK_dbo.Transportista_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[Transportista]([Localidad_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[Transportista]([Provincia_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);




