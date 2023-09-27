CREATE TABLE [dbo].[AsignacionAutomatismoGranoEnRecorrido] (
  
  [Id] INT IDENTITY (1, 1) NOT NULL,

  [Recorrido_Id] INT NOT NULL,

  [CallePreBalanza_Id] INT NOT NULL,

  [CallePreHidraulica_Id] INT NOT NULL,

  CONSTRAINT [PK_AsignacionAutomatismoGranoEnRecorrido] PRIMARY KEY ([Id] ASC),
  
  CONSTRAINT [FK_dbo.AsignacionAutomatismoGranoEnRecorrido_Recorrido_Id]
    FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido](Id) ON DELETE CASCADE,

  CONSTRAINT [FK_dbo.AsignacionAutomatismoGranoEnRecorrido_CallePreBalanza_Id]
    FOREIGN KEY ([CallePreBalanza_Id]) REFERENCES [dbo].[Calle](Id),

  CONSTRAINT [FK_dbo.AsignacionAutomatismoGranoEnRecorrido_CallePreHidraulica_Id] 
    FOREIGN KEY (CallePreHidraulica_Id) REFERENCES [dbo].[Calle](Id),
);