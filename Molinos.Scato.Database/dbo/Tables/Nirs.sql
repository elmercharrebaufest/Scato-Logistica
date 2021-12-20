CREATE TABLE [dbo].[Nirs] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (30)  NOT NULL,
    [Codigo]           NVARCHAR (30)  NOT NULL,
    [DescripcionCorta] NVARCHAR (8)   NOT NULL,
    [Modalidad]        INT            NOT NULL,
	[Centro_Id]        INT            NOT NULL,
	[PuestoDeTrabajo]	NVARCHAR(50)	NULL, 
    CONSTRAINT [PK_dbo.Nirs] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Nirs_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])

);

GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Nirs]([Centro_Id] ASC);