CREATE TABLE [dbo].AutomatismoTipoLlamado (
    [Id]	INT  IDENTITY (1, 1) NOT NULL,
	[Codigo]	NVARCHAR (3) NOT NULL,
	[Descripcion]	NVARCHAR(50) NOT NULL,
    [Activo] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.AutomatismoTipoLlamado] PRIMARY KEY CLUSTERED ([Id] ASC)
);

