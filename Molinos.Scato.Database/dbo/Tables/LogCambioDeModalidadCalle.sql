CREATE TABLE [dbo].[LogCambioDeModalidadCalle] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Motivo]     NVARCHAR (200) DEFAULT ((1)) NOT NULL,
    [Usuario]    NVARCHAR (40)  NOT NULL,
    [Fecha]      DATETIME       NOT NULL,
    [Calle_Id]   INT            NOT NULL,
    [Automatico] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.LogCambioDeModalidadCalle] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.LogCambioDeModalidadCalle_dbo.Calle_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id])
);

