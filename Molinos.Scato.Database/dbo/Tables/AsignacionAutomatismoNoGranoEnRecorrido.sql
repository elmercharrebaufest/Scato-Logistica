CREATE TABLE [dbo].[AsignacionAutomatismoNoGranoEnRecorrido] (
  
  [Id] INT IDENTITY (1, 1) NOT NULL,

  [Recorrido_Id] INT NOT NULL,

  [CallePlanta_Id] INT NOT NULL,

  CONSTRAINT [PK_AsignacionAutomatismoNoGranoEnRecorrido] PRIMARY KEY ([Id] ASC),
  
  CONSTRAINT [FK_dbo.AsignacionAutomatismoNoGranoEnRecorrido_Recorrido_Id]
    FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido](Id) ON DELETE CASCADE,

  CONSTRAINT [FK_dbo.AsignacionAutomatismoNoGranoEnRecorrido_CallePreBalanza_Id]
    FOREIGN KEY ([CallePlanta_Id]) REFERENCES [dbo].[Calle](Id),
);