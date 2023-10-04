CREATE TABLE [dbo].[AutomatismoNoGrano]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [CallePlanta_Id] INT NOT NULL, 
    [CallePlayaInterna_Id] INT NOT NULL,
    [Almacen_Id] INT NOT NULL,
    [PuntoDeCarga_Id] INT NOT NULL,

    [Activo] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_dbo.AutomatismoNoGrano_CallePlanta_Id]
    FOREIGN KEY (CallePlanta_Id) REFERENCES [dbo].[Calle](Id),
    CONSTRAINT [FK_dbo.AutomatismoNoGrano_CallePlayaInterna_Id]
    FOREIGN KEY (CallePlayaInterna_Id) REFERENCES [dbo].[Calle](Id),
    CONSTRAINT [FK_dbo.AutomatismoNoGrano_Almacen_Id]
    FOREIGN KEY (Almacen_Id) REFERENCES [dbo].[Almacen](Id),
    CONSTRAINT [FK_dbo.AutomatismoNoGrano_PuntoDeCarga_Id]
    FOREIGN KEY (PuntoDeCarga_Id) REFERENCES [dbo].[PuntoDeCarga](Id), 
    CONSTRAINT [PK_AutomatismoNoGrano] PRIMARY KEY ([Id]),
)
