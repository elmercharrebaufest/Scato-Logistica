CREATE TABLE [dbo].[MotivoReasignacionDeTarjeta] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (30) NOT NULL,
    [DescripcionCorta] NVARCHAR (8) NOT NULL,
    CONSTRAINT [PK_dbo.MotivoReasignacionDeTarjeta] PRIMARY KEY CLUSTERED ([Id] ASC)
);

