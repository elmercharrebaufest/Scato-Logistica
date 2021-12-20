CREATE TABLE [dbo].[LineUp] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[Embarque_Id]      INT NOT NULL,
	[Recorrido_Id]     INT NOT NULL,
	[PlanoDeCarga_Id]      INT NOT NULL,
	[ModuloDeCarga_Id]      INT NULL,
	[CartaDeSubidaEnviada]		bit NOT NULL default 0,
	[CartaDeSubidaAprobada]     datetime2 NULL,
	[CargaEnSap]				bit NOT NULL default 0,
	[NominacionDePractico]		bit NOT NULL default 0,
	[SeguridadPortuaria]		bit NOT NULL default 0,
	[InspeccionSenasa]			bit NOT NULL default 0,
	[ControlSenasa]				bit NOT NULL default 0,
	[ControlPrivado]			bit NOT NULL default 0,
	[Amarrador]					bit NOT NULL default 0,
	[AgenciaContactada]			bit NOT NULL default 0,
	[PlanoDeCargaEnviado]					bit NOT NULL default 0,
	[Orden]			decimal(18,4) NOT NULL default 0,
    CONSTRAINT [PK_dbo.LineUp] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.LineUp_dbo.Embarque_Embarque_Id] FOREIGN KEY ([Embarque_Id]) REFERENCES [dbo].[Embarque] ([Id]),
	CONSTRAINT [FK_dbo.LineUp_dbo.PlanoDeCarga_PlanoDeCarga_Id] FOREIGN KEY ([PlanoDeCarga_Id]) REFERENCES [dbo].[PlanoDeCarga] ([Id]),
	CONSTRAINT [FK_dbo.LineUp_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]),
	CONSTRAINT [FK_dbo.LineUp_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_Embarque_Id]
    ON [dbo].[LineUp]([Embarque_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[LineUp]([Recorrido_Id] ASC);