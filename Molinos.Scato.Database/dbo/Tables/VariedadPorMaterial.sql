CREATE TABLE [dbo].[VariedadPorMaterial] (

  [Id] INT IDENTITY (1, 1) NOT NULL,

  [Variedad_Id] INT NOT NULL,

  [Material_Id] INT NOT NULL,
  
  CONSTRAINT [PK_VariedadPorMaterial] PRIMARY KEY ([Id] ASC),
  CONSTRAINT [FK_VariedadPorMaterial_VariedadMaterial] FOREIGN KEY (Variedad_Id) REFERENCES [dbo].[VariedadMaterial](Id),
  CONSTRAINT [FK_VariedadPorMaterial_Material] FOREIGN KEY (Material_Id) REFERENCES [dbo].[Material](Id)

);