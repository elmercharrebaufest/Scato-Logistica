CREATE TABLE [dbo].[AutomatismoGrano] (

  [Id] INT IDENTITY (1, 1) NOT NULL,

  [Material_Id] INT NOT NULL,

  [CamionEscalable] BIT NOT NULL DEFAULT 0,

  [CallePreBalanza_Id] INT NOT NULL,

  [AplicaFiltroCalidad] BIT NOT NULL DEFAULT 0,

  [Calidad_Id] INT NULL,

  [Minimo] DECIMAL(10,2) NULL,

  [Maximo] DECIMAL(10,2) NULL,

  [CallePreHidraulica_Id] INT NOT NULL,

  [Almacen_Id] INT NOT NULL,

  [Activo] BIT NOT NULL

  CONSTRAINT [PK_AutomatismoGrano] PRIMARY KEY ([Id] ASC),

  CONSTRAINT [FK_dbo.AutomatismoGrano_CallePreBalanza_Id]
    FOREIGN KEY ([CallePreBalanza_Id]) REFERENCES [dbo].[Calle](Id),

  CONSTRAINT [FK_dbo.AutomatismoGrano_Calidad_Id] 
    FOREIGN KEY (Calidad_Id) REFERENCES [dbo].[CaracteristicaDeCalidad](Id),

  CONSTRAINT [FK_dbo.AutomatismoGrano_CallePreHidraulica_Id] 
    FOREIGN KEY (CallePreHidraulica_Id) REFERENCES [dbo].[Calle](Id),

  CONSTRAINT [FK_dbo.AutomatismoGrano_Almacen_Id]
    FOREIGN KEY (Almacen_Id) REFERENCES [dbo].[Almacen](Id)
);