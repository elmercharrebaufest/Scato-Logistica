CREATE TABLE [dbo].[TipoVariedad] (

  [Id] INT IDENTITY (1, 1),

  [Descripcion] VARCHAR(100) NOT NULL,
  
  [Codigo] VARCHAR(10) NULL,  

  [Activo] BIT NOT NULL DEFAULT 1,

  [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),

  [FechaModificacion] DATETIME NULL,

  [CreadoPor] VARCHAR(50) NOT NULL,

  [ModificadoPor] VARCHAR(50) NULL,

  CONSTRAINT [PK_TipoVariedad] PRIMARY KEY CLUSTERED ([Id] ASC)

);