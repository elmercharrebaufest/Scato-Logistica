CREATE TABLE [dbo].[Carga] (
    [Id]                         INT           NOT NULL,
    [NumeroBalanza]              NVARCHAR (50) NOT NULL,
    [Vapor_Id]                   INT           NOT NULL,
    [Material_Id]                INT           NOT NULL,
    [Bodega_Id]                  INT           NOT NULL,
    [Exportador_Id]              INT           NOT NULL,
    [Destino_Id]                 INT           NOT NULL,
    [PesoProgramado]             INT           NULL,
    [ToneladasAW]                INT           NULL,
    [FechaInicio]                DATETIME      NULL,
    [CargaOpuesta_Id]            INT           NULL,
    [CargaOpuesta_NumeroBalanza] NVARCHAR (50) NULL,
    CONSTRAINT [PK_dbo.Carga] PRIMARY KEY CLUSTERED ([Id] ASC, [NumeroBalanza] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Carga_dbo.Bodega_Bodega_Id] FOREIGN KEY ([Bodega_Id]) REFERENCES [dbo].[Bodega] ([Id]),
    CONSTRAINT [FK_dbo.Carga_dbo.Carga_CargaOpuesta_Id] FOREIGN KEY ([CargaOpuesta_Id], [CargaOpuesta_NumeroBalanza]) REFERENCES [dbo].[Carga] ([Id], [NumeroBalanza]),
    CONSTRAINT [FK_dbo.Carga_dbo.Destino_Destino_Id] FOREIGN KEY ([Destino_Id]) REFERENCES [dbo].[Destino] ([Id]),
    CONSTRAINT [FK_dbo.Carga_dbo.Exportador_Exportador_Id] FOREIGN KEY ([Exportador_Id]) REFERENCES [dbo].[Exportador] ([Id]),
    CONSTRAINT [FK_dbo.Carga_dbo.MaterialPuerto_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[MaterialPuerto] ([Id]),
    CONSTRAINT [FK_dbo.Carga_dbo.Vapor_Vapor_Id] FOREIGN KEY ([Vapor_Id]) REFERENCES [dbo].[Vapor] ([Id])
);


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO

