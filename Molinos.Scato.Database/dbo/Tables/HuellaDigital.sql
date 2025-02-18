CREATE TABLE [dbo].[HuellaDigital]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Estado] BIT NOT NULL, 
    [Patente] NVARCHAR(60) NOT NULL, 
    [Acoplado] NVARCHAR(60) NULL, 
    [PesoTara] INT NOT NULL, 
    [FechaHoraPesaje] DATETIME NOT NULL, 
    [Usuario] NVARCHAR(40) NOT NULL, 
    [Observaciones] NVARCHAR(250) NOT NULL,
    [Chofer_Id] INT NOT NULL, 
    [Transportista_Id] INT NOT NULL,
    [Centro_Id] INT NOT NULL, 
    [Balanza_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.HuellaDigital] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.HuellaDigital_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.HuellaDigital_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
    CONSTRAINT [FK_dbo.HuellaDigital_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.HuellaDigital_dbo.BalanzaTara_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id])
    
)
