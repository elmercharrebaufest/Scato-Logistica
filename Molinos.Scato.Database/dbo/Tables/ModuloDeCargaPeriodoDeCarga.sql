CREATE TABLE [dbo].[ModuloDeCargaPeriodoDeCarga]
(
	[Id]                        INT IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]          INT NOT NULL,
	[FechaAmarro]				datetime NULL,
	[HoraAmarro]				nvarchar(8) NULL,
	[VientoAmarro]				nvarchar(20) NULL,
	[DireccionAmarro]			nvarchar(20) NULL,
	[FechaDesamarro]			datetime NULL,
	[HoraDesamarro]				nvarchar(8) NULL,
	[VientoDesamarro]			nvarchar(20) NULL,
	[DireccionDesamarro]		nvarchar(20) NULL,
	[FechaHabilitacion]			datetime NULL,
	[HoraHabilitacion]			nvarchar(8) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaPeriodoDeCarga] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ModuloDeCargaPeriodoDeCarga_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) on delete cascade
);