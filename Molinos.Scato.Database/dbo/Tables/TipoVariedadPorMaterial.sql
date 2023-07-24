CREATE TABLE [dbo].[TipoVariedadPorMaterial] (

  [Id] INT IDENTITY (1, 1) NOT NULL,

  [TipoVariedad_Id] INT NOT NULL,

  [Material_Id] INT NOT NULL,
  
  CONSTRAINT [PK_TipoVariedadPorMaterial] PRIMARY KEY ([Id] ASC),
  CONSTRAINT [FK_TipoVariedadPorMaterial_TipoVariedad] FOREIGN KEY (TipoVariedad_Id) REFERENCES [dbo].[TipoVariedad](Id),
  CONSTRAINT [FK_TipoVariedadPorMaterial_Material] FOREIGN KEY (Material_Id) REFERENCES [dbo].[Material](Id)

);
