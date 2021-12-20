CREATE TABLE [dbo].[CaracteristicaMaterial] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,
    [MaterialId] INT NOT NULL,
    CONSTRAINT [PK_dbo.CaracteristicaMaterial] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CaracteristicaMaterial_dbo.Material_Id] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([Id]),
);

