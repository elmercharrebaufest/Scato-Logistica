CREATE TABLE [dbo].[AnalisisObligatorio] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[Material_Id]			INT            NOT NULL,
	[Centro_Id]			INT            NOT NULL,
    [IntervaloDeAnalisis] INT NOT NULL, 
    [UltimoAnalisis] DATETIME NULL,
    [AlertarAnalisisIntervalo] INT NOT NULL DEFAULT 5, 
    CONSTRAINT [PK_dbo.AnalisisObligatorio] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.AnalisisObligatorio_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.AnalisisObligatorio_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[AnalisisObligatorio]([Material_Id] ASC);
GO