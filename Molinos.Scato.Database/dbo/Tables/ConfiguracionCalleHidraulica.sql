CREATE TABLE [dbo].[ConfiguracionCalleHidraulica]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Calle_Id] INT NOT NULL UNIQUE, 
    [CodigoCartel] NVARCHAR(50) NULL,
    [CodigoSensorCamaraALPR] NVARCHAR(50) NULL, 
    [CodigoSensorCirculacion] NVARCHAR(50) NULL, 
    [CodigoCamaraALPR] NVARCHAR(50) NULL, 
    CONSTRAINT [PK_dbo.ConfiguracionCalleHidraulica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ConfiguracionCalleHidraulica_dbo.Calle_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id])
)
