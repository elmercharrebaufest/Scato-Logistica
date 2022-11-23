CREATE TABLE [dbo].[ConfirmacionCargaDescarga] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [NombreUsuario]        NVARCHAR (20) NOT NULL,
    [FechaConfirmacion]        DATETIME      NOT NULL,
    [Recorrido_Id]              INT           NULL
    CONSTRAINT [PK_dbo.ConfirmacionCargaDescarga] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.ConfirmacionCargaDescarga_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].[Recorrido] ([Id])
);