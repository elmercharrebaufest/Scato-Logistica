CREATE TABLE [dbo].[Variedad] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[Descripcion]         NVARCHAR(40) NOT NULL,
	[NumeroINV] NVARCHAR(8) NOT NULL, 
    CONSTRAINT [PK_dbo.Variedad] PRIMARY KEY CLUSTERED ([Id] ASC),
);