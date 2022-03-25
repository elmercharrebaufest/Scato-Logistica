CREATE TABLE [dbo].[MaterialPuertoCantidad] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [Cantidad]          INT          NULL,
    [Embarque_Id]       INT          NOT NULL,
    [MaterialPuerto_Id] INT          NOT NULL,
    [Color]             NVARCHAR (7) DEFAULT ('#000000') NOT NULL,
    CONSTRAINT [PK_MaterialPuertoCantidad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MaterialPuertoCantidad_dbo.Embarque_Embarque_Id] FOREIGN KEY ([Embarque_Id]) REFERENCES [dbo].[Embarque] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MaterialPuertoCantidad_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id])
);

