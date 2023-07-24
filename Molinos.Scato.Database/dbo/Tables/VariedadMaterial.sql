CREATE TABLE [dbo].[VariedadMaterial] (

  [Id] INT IDENTITY (1, 1) PRIMARY KEY,

  [Descripcion] VARCHAR(100) NOT NULL,
  
  [Codigo] VARCHAR(10) NULL,  

  [Activo] BIT NOT NULL DEFAULT 1,

  [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),

  [FechaModificacion] DATETIME NULL,

  [CreadoPor] VARCHAR(50) NOT NULL,

  [ModificadoPor] VARCHAR(50) NULL

);