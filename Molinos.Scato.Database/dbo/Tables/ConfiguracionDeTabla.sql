CREATE TABLE [dbo].[ConfiguracionDeTabla] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Centro_Id]			INT NOT NULL,
    [Material_Id] INT NOT NULL,
	[Usuario_Id] INT NULL,
    CONSTRAINT [PK_dbo.ConfiguracionDeTabla] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.ConfiguracionDeTabla_dbo.Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
	CONSTRAINT [FK_dbo.ConfiguracionDeTabla_dbo.Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
	CONSTRAINT [FK_dbo.ConfiguracionDeTabla_dbo.Usuario_Id] FOREIGN KEY ([Usuario_Id]) REFERENCES [dbo].[Usuario] ([Id])
);

