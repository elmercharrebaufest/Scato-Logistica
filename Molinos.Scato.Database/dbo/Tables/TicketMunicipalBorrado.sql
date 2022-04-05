CREATE TABLE [dbo].[TicketMunicipalBorrado] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [CentroId]      INT            NULL,
    [MaterialId]    INT            NULL,
    [Fecha]         DATETIME       NULL,
    [NroTarjeta]    NVARCHAR (50)  NULL,
    [Patente]       NVARCHAR (50)  NULL,
    [TipoDocumento] INT            NULL,
    [NroDocumento]  NVARCHAR (50)  NULL,
    [ChoferNombre]  NVARCHAR (100) NULL,
    [Recibo]        NVARCHAR (50)  NULL,
    [ChoferCuil]    NVARCHAR (50)  NULL,
    CONSTRAINT [PK_dbo.TicketMunicipalBorrado] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.TicketMunicipalBorrado_dbo.Centro_CentroId] FOREIGN KEY ([CentroId]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.TicketMunicipalBorrado_dbo.Material_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Material] ([Id])
);

