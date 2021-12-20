CREATE TABLE [dbo].[LogCambioDeModalidadCalle] (
    [Id]                    INT              IDENTITY (1, 1) NOT NULL,
    [Motivo]				NVARCHAR (200)    NOT NULL default 1,
    [Usuario]				NVARCHAR (40)   NOT NULL,
	[Fecha]					DATETIME NOT NULL,
	[Calle_Id]				INT NOT NULL,
	
    [Automatico] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.LogCambioDeModalidadCalle] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.LogCambioDeModalidadCalle_dbo.Calle_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id])
);