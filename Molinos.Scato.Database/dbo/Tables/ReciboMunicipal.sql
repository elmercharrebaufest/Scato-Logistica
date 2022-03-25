CREATE TABLE [dbo].[ReciboMunicipal] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [Monto]           DECIMAL (18, 2) NOT NULL,
    [Ordenanza]       NVARCHAR (15)   NULL,
    [Centro_Id]       INT             NOT NULL,
    [FechaActivacion] DATETIME        DEFAULT (((18)/(1))/(2019)) NOT NULL,
    [TipoVehiculo]    INT             NULL,
    CONSTRAINT [PK_dbo.ReciboMunicipal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ReciboMunicipal_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[ReciboMunicipal]([Centro_Id] ASC);