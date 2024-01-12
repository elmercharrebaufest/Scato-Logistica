CREATE TABLE [dbo].[TipoVariedadPorMaterial] (

  [Id] INT IDENTITY (1, 1) NOT NULL,

  [TipoVariedad_Id] INT NOT NULL,

  [Material_Id] INT NOT NULL,

  [ColorFondo] VARCHAR(10) NULL,

  [ColorTexto] VARCHAR(10) NULL,

  [Borrado] BIT NULL DEFAULT 0,

  [CreadoPor] VARCHAR(50) NULL,
  
  [FechaCreacion] DATETIME NULL DEFAULT GETDATE(),
  
  [ModificadoPor] VARCHAR(50) NULL,
  
  [FechaModificacion] DATETIME NULL,

  CONSTRAINT [PK_TipoVariedadPorMaterial] PRIMARY KEY ([Id] ASC),
  CONSTRAINT [FK_TipoVariedadPorMaterial_TipoVariedad] FOREIGN KEY (TipoVariedad_Id) REFERENCES [dbo].[TipoVariedad](Id),
  CONSTRAINT [FK_TipoVariedadPorMaterial_Material] FOREIGN KEY (Material_Id) REFERENCES [dbo].[Material](Id)

);

GO

CREATE UNIQUE INDEX IX_TipoVariedadPorMaterial_TipoVariedadId_MaterialId
  ON [dbo].[TipoVariedadPorMaterial](TipoVariedad_Id, Material_Id);
