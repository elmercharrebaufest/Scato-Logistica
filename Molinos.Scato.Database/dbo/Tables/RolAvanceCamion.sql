CREATE TABLE [dbo].[RolAvanceCamion]
(
    [Id]                 INT            IDENTITY(1,1) NOT NULL,
    [CodigoRol]          NVARCHAR(200)  NOT NULL,
    [PuestoDeTrabajo_Id] INT            NULL,

    CONSTRAINT [PK_dbo.RolAvanceCamion]
        PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_dbo.RolAvanceCamion_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id]
        FOREIGN KEY ([PuestoDeTrabajo_Id])
        REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);
GO
