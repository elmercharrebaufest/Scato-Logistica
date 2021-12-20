CREATE TABLE [dbo].[PagoConMercadoPago] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Recorrido_Id] INT            NOT NULL,
    [MercadoPagoId]         NVARCHAR (50) NULL,
    [Estado]    NVARCHAR (30) NOT NULL,
    [DetalleDelEstado] NVARCHAR (100)           NULL,
    [MontoCobrado]  DECIMAL(8, 2) NULL,
    [Fecha] DATETIME2 NOT NULL, 
	[Devuelto] bit NOT NULL DEFAULT 0,
	[Token] NVARCHAR (200)           NULL,
    CONSTRAINT [PK_dbo.PagoConMercadoPago] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PagoConMercadoPago_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Id]
    ON [dbo].[PagoConMercadoPago]([Recorrido_Id] ASC);



