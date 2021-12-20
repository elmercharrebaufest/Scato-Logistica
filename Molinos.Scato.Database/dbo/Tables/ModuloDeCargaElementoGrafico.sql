CREATE TABLE [dbo].[ModuloDeCargaElementoGrafico]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
    [CeldaManoDeEmbarque_Id]    INT NULL,
    [Tipo]                      NVARCHAR(15) NULL,
    [X]                         FLOAT NULL,
    [Y]                         FLOAT NULL,
    [Forma]                     NVARCHAR(15) NULL,
    [Width]                     FLOAT NULL,
    [Height]                    FLOAT NULL,
    [RadioX]                    FLOAT NULL,
    [RadioY]                    FLOAT NULL,
    [MaterialPuerto_Id]         INT NULL,
    [Rotacion]                  BIT NOT NULL default 0,
    CONSTRAINT [PK_dbo.ModuloDeCargaElementoGrafico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGrafico_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade,
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGrafico_dbo.CeldaManoDeEmbarque_CeldaManoDeEmbarque_Id] FOREIGN KEY ([CeldaManoDeEmbarque_Id]) REFERENCES [dbo].[CeldaManoDeEmbarque] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaElementoGrafico_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id])
);