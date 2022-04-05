CREATE TABLE [dbo].[LineUp] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [Embarque_Id]           INT             NOT NULL,
    [Recorrido_Id]          INT             NOT NULL,
    [PlanoDeCarga_Id]       INT             NOT NULL,
    [ModuloDeCarga_Id]      INT             NULL,
    [CartaDeSubidaEnviada]  BIT             DEFAULT ((0)) NOT NULL,
    [CartaDeSubidaAprobada] DATETIME2 (7)   NULL,
    [CargaEnSap]            BIT             DEFAULT ((0)) NOT NULL,
    [NominacionDePractico]  BIT             DEFAULT ((0)) NOT NULL,
    [SeguridadPortuaria]    BIT             DEFAULT ((0)) NOT NULL,
    [InspeccionSenasa]      BIT             DEFAULT ((0)) NOT NULL,
    [ControlSenasa]         BIT             DEFAULT ((0)) NOT NULL,
    [ControlPrivado]        BIT             DEFAULT ((0)) NOT NULL,
    [Amarrador]             BIT             DEFAULT ((0)) NOT NULL,
    [AgenciaContactada]     BIT             DEFAULT ((0)) NOT NULL,
    [PlanoDeCargaEnviado]   BIT             DEFAULT ((0)) NOT NULL,
    [Orden]                 DECIMAL (18, 4) DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.LineUp] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LineUp_dbo.Embarque_Embarque_Id] FOREIGN KEY ([Embarque_Id]) REFERENCES [dbo].[Embarque] ([Id]),
    CONSTRAINT [FK_dbo.LineUp_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]),
    CONSTRAINT [FK_dbo.LineUp_dbo.PlanoDeCarga_PlanoDeCarga_Id] FOREIGN KEY ([PlanoDeCarga_Id]) REFERENCES [dbo].[PlanoDeCarga] ([Id]),
    CONSTRAINT [FK_dbo.LineUp_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);



GO
CREATE NONCLUSTERED INDEX [IX_Embarque_Id]
    ON [dbo].[LineUp]([Embarque_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[LineUp]([Recorrido_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);

