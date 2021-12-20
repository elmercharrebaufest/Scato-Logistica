CREATE TABLE [dbo].[AjusteDeCalidad] (
    [Id]                     INT              IDENTITY (1, 1) NOT NULL,
    [TipoDocumentoIngreso]   INT              NOT NULL,
    [NumeroDocumentoIngreso] NVARCHAR (40)   NOT NULL,
    [ValorOriginal]           DECIMAL (18, 2) NOT NULL,
    [ValorNuevo]            DECIMAL (18, 2) NOT NULL,
	[Fecha]	         DATETIME NOT NULL,
	[Usuario]	         NVARCHAR (30) NOT NULL,
	[CaracteristicaDeCalidad_Id] INT NOT NULL,  
    CONSTRAINT [FK_dbo.AjusteDeCalidad_dbo.CaracteristicaDeCalidad_CaracteristicaDeCalidad_Id] FOREIGN KEY ([CaracteristicaDeCalidad_Id]) REFERENCES [dbo].[CaracteristicaDeCalidad] ([Id]),
    CONSTRAINT [PK_dbo.AjusteDeCalidad] PRIMARY KEY CLUSTERED ([Id] ASC),
);