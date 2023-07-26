CREATE TABLE [dbo].[TipoVariedad] (

  [Id] INT IDENTITY (1, 1),

  [Descripcion] VARCHAR(100) NOT NULL,
  
  [Codigo] VARCHAR(10) NULL,  

  [Borrado] BIT NULL DEFAULT 0,

  [CreadoPor] VARCHAR(50) NULL,
  
  [FechaCreacion] DATETIME NULL DEFAULT GETDATE(),
  
  [ModificadoPor] VARCHAR(50) NULL,
  
  [FechaModificacion] DATETIME NULL,

  CONSTRAINT [PK_TipoVariedad] PRIMARY KEY CLUSTERED ([Id] ASC)

);