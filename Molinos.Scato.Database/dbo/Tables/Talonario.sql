CREATE TABLE [dbo].[Talonario](
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[Descripcion]		NVARCHAR(30) NOT NULL,
	[Sucursal]			INT NOT NULL,
	[PrimerNumero]		INT NOT NULL,
	[UltimoNumero]		INT NOT NULL,
	[ProximoNumero]		INT NOT NULL,
	[Centro_Id]			INT NOT NULL,

 CONSTRAINT [PK_Talonario] PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [FK_dbo.Talonario_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
 );


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Talonario]([Centro_Id] ASC);