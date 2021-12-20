CREATE TABLE [dbo].[BalanzaPuerto]
(
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
	[UltimaValidacion] INT NOT NULL, 
    [CodigoBalanza] NVARCHAR(50) NOT NULL, 
    [CodigoDispositivo] NVARCHAR(50) NULL,
	[Centro_Id] INT  NULL,
	[Administrativa] BIT NOT NULL DEFAULT 0, 
    [OffSetPlc] INT NOT NULL DEFAULT 0, 
    [IntentosValidacion] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_dbo.BalanzaPuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.BalanzaPuerto_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
)
