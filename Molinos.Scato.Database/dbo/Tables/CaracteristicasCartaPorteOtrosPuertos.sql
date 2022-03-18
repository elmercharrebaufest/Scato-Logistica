CREATE TABLE [dbo].[CaracteristicasCartaPorteOtrosPuertos] (
    [Id]                        INT             IDENTITY (1, 1) NOT NULL,
    [CartaPorteOtrosPuertos_Id] INT             NOT NULL,
    [CodigoExterno]             NVARCHAR (50)   NOT NULL,
    [Valor]                     DECIMAL (15, 5) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


