CREATE TABLE [dbo].[Casillero] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Numero] NVARCHAR(11) NOT NULL,
    [Capacidad]   INT            NOT NULL,
	[Centro_Id]			 INT  NOT NULL,
    CONSTRAINT [PK_dbo.Casillero] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Casillero_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);
GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Casillero]([Centro_Id] ASC);
