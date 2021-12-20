CREATE TABLE [dbo].[CallePorRecorrido] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Calle_Id] INT NOT NULL,
    [Recorrido_Id] INT NULL,
    [FechaIngeso]	         DATETIME NULL,
    [FechaEgreso] DATETIME NULL,
    [CargaDeCupo_Id] INT NULL,
    [UltimoDeLaFila] bit not NULL default 0,
    CONSTRAINT [PK_dbo.CallePorRecorrido] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.CallePorRecorrido_dbo.CallePorRecorrido_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id]),
    CONSTRAINT [FK_dbo.CallePorRecorrido_dbo.CallePorRecorrido_CargaDeCupo_Id] FOREIGN KEY ([CargaDeCupo_Id]) REFERENCES [dbo].[CargaDeCupo] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.CallePorRecorrido_dbo.CallePorRecorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id])
);

GO 
CREATE NONCLUSTERED INDEX [IX_CallePorRecorrido_FechaEgreso_Calle]
ON [dbo].[CallePorRecorrido] ([FechaEgreso],[Calle_Id])