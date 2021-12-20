CREATE TABLE [dbo].[ExcepcionAlControl] (
    [Id]               INT      IDENTITY (1, 1) NOT NULL,
    [FechaDesde]       DATETIME NOT NULL,
    [FechaHasta]       DATETIME NOT NULL,
	[FechaDeCarga]     DATETIME NOT NULL,
	[Usuario]     varchar(20) NOT NULL,
    [Transportista_Id] INT      NOT NULL,
    [Material_Id]      INT      NOT NULL,
    [Centro_Id]        INT      NOT NULL,
	[Motivo]		   INT      NOT NULL DEFAULT 0,
    [CentroDestino_Id] INT NULL, 
    [ClienteDestino_Id] INT NULL, 
    [TipoDestino] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.ExcepcionAlControl] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ExcepcionAlControl_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControl_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.ExcepcionAlControl_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionAlControl_dbo.Centro_CentroDestino_Id] FOREIGN KEY ([CentroDestino_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [FK_dbo.ExcepcionAlControl_dbo.Cliente_ClienteDestino_Id] FOREIGN KEY ([ClienteDestino_Id]) REFERENCES [dbo].[Cliente] ([Id])

);


GO
CREATE NONCLUSTERED INDEX [IX_Transportista_Id]
    ON [dbo].[ExcepcionAlControl]([Transportista_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[ExcepcionAlControl]([Material_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[ExcepcionAlControl]([Centro_Id] ASC);

