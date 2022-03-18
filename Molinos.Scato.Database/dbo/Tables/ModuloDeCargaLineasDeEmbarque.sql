CREATE TABLE [dbo].[ModuloDeCargaLineasDeEmbarque] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [ModuloDeCarga_Id]   INT           NOT NULL,
    [Linea]              NVARCHAR (10) NULL,
    [MaterialPuerto_Id]  INT           NULL,
    [TkInicial]          NVARCHAR (10) NULL,
    [TemperaturaInicial] FLOAT (53)    NULL,
    [AlturaInicialCM]    FLOAT (53)    NULL,
    [AlturaInicialMM]    FLOAT (53)    NULL,
    [DensidadInicial]    FLOAT (53)    NULL,
    [TemperaturaFinal]   FLOAT (53)    NULL,
    [Litros]             FLOAT (53)    NULL,
    [DensidadFinal]      FLOAT (53)    NULL,
    [AlturaFinalCM]      FLOAT (53)    NULL,
    [AlturaFinalMM]      FLOAT (53)    NULL,
    [Kilos]              FLOAT (53)    NULL,
    [TkFinal]            NVARCHAR (10) NULL,
    CONSTRAINT [PK_dbo.ModuloDeCargaLineasDeEmbarque] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ModuloDeCargaLineasDeEmbarque_dbo.MaterialPuerto_MaterialPuerto_Id] FOREIGN KEY ([MaterialPuerto_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.ModuloDeCargaLineasDeEmbarque_dbo.ModuloDeCarga_ModuloDeCarga_Id] FOREIGN KEY ([ModuloDeCarga_Id]) REFERENCES [dbo].[ModuloDeCarga] ([Id]) ON DELETE CASCADE
);

