CREATE TABLE [dbo].[MuestraDeInase] (
    [Id]                   INT  IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id]         INT                  NOT NULL,
    [FechaMuestra]         DATETIME             NOT NULL,
    [MuestraEnviada]       BIT                  NOT NULL,
    [LoteInase_Id] INT NULL, 
    CONSTRAINT [PK_dbo.MuestraDeInase] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MuestraDeInase_dbo.Calado_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.MuestraDeInase_dbo.LoteInase_LoteInase_Id] FOREIGN KEY ([LoteInase_Id]) REFERENCES [dbo].[LoteInase] ([Id]),
);

