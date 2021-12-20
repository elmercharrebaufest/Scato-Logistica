CREATE TABLE [dbo].[Contingencia] (
    [Id]                    INT IDENTITY (1, 1) NOT NULL,
    [TipoContingencia]		NVARCHAR(50)       NOT NULL,
    [Fecha]                 DATETIME NOT NULL,
	[Usuario]               NVARCHAR(20) NOT NULL,
    [Activado]              BIT NOT NULL,
	[Motivo]                NVARCHAR(500) NOT NULL,
    CONSTRAINT [PK_dbo.Contingencia] PRIMARY KEY CLUSTERED ([Id] ASC)
);

