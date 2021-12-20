CREATE TABLE [dbo].[MuestraEnvioACamaraBiotecnologia] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id]         INT NOT NULL,
	[LoteBiotecnologia_Id]         INT NOT NULL,
	CONSTRAINT [PK_dbo.MuestraEnvioACamaraBiotecnologia] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.MuestraEnvioACamaraBiotecnologia_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_dbo.MuestraEnvioACamaraBiotecnologia_dbo.LoteBiotecnologia_LoteBiotecnologia_Id] FOREIGN KEY ([LoteBiotecnologia_Id]) REFERENCES [dbo].[LoteBiotecnologia] ([Id]),
	
);