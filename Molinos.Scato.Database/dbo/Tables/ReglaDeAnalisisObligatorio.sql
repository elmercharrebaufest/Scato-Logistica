CREATE TABLE [dbo].[ReglaDeAnalisisObligatorio] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[Provincia_Id]			INT            NULL,
	[Localidad_Id]			INT            NULL,
	[Material_Id]			INT            NOT NULL,
    [Cosecha]   NVARCHAR(10) NOT NULL,
	[FechaDeVigenciaHasta]   DATETIME NOT NULL,
    [CantidadAnalisis] INT NOT NULL, 
	[Centro_Id] INT NULL,
    CONSTRAINT [PK_dbo.ReglaDeAnalisisObligatorio] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ReglaDeAnalisisObligatorio_dbo.Provincia_Provincia_Id] FOREIGN KEY ([Provincia_Id]) REFERENCES [dbo].[Provincia] ([Id]),
	CONSTRAINT [FK_dbo.ReglaDeAnalisisObligatorio_dbo.Localidad_Localidad_Id] FOREIGN KEY ([Localidad_Id]) REFERENCES [dbo].[Localidad] ([Id]),
	CONSTRAINT [FK_dbo.ReglaDeAnalisisObligatorio_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.ReglaDeAnalisisObligatorio_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[ReglaDeAnalisisObligatorio]([Material_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Localidad_Id]
    ON [dbo].[ReglaDeAnalisisObligatorio]([Localidad_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Provincia_Id]
    ON [dbo].[ReglaDeAnalisisObligatorio]([Provincia_Id] ASC);
GO