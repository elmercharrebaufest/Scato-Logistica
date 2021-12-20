CREATE TABLE [dbo].[RegistroBalanzaPuerto] (
    [Id]     INT NOT NULL,
    [NumeroBalanza] NVARCHAR(50) NOT NULL, 
    [Tipo] NVARCHAR(30) NOT NULL, 
    [Fecha] DATETIME NOT NULL,
	[EnviadoASap] BIT NOT NULL DEFAULT 0,
	
    CONSTRAINT [PK_dbo.RegistroBalanzaPuerto] PRIMARY KEY CLUSTERED ([Id] ASC,[NumeroBalanza]),
);
GO

CREATE NONCLUSTERED INDEX IX_RegistroBalanzaPuerto_Tipo_Fecha
ON [dbo].[RegistroBalanzaPuerto] ([Tipo],[Fecha])
INCLUDE ([Id],[NumeroBalanza])
GO