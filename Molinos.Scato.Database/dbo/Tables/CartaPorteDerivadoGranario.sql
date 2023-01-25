CREATE TABLE [dbo].[CartaPorteDerivadoGranario] (
    [Id] INT NOT NULL IDENTITY,
    [NroCTG] CHAR(12) NULL, 
    [Sucursal] CHAR(5) NULL, 
    [NroOrden] CHAR(8) NULL, 
    [RutaFotoCPEDG] VARCHAR(200) NULL,
    [Recorrido_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.CartaPorteDerivadoGranario] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CartaPorteDerivadoGranario_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);


