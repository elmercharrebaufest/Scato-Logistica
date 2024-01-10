CREATE TABLE [dbo].[AsignacionNoGranoEnRecorrido] (
  
  [Id] INT IDENTITY (1, 1) NOT NULL,

  [Recorrido_Id] INT NOT NULL,

  [CallePlanta_Id] INT NULL,

  [AplicaConteo] BIT NOT NULL DEFAULT 0,

  CONSTRAINT [PK_AsignacionNoGranoEnRecorrido] PRIMARY KEY ([Id] ASC),
  
  CONSTRAINT [FK_dbo.AsignacionNoGranoEnRecorrido_Recorrido_Id]
    FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido](Id) ON DELETE CASCADE,

  CONSTRAINT [FK_dbo.AsignacionNoGranoEnRecorrido_CallePlanta_Id]
    FOREIGN KEY ([CallePlanta_Id]) REFERENCES [dbo].[Calle](Id)
);