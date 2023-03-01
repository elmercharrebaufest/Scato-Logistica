CREATE TABLE [dbo].[Domicilio] (
    [Id]                               INT            IDENTITY (1, 1) NOT NULL,
    [Tipo]                        INT  NOT NULL,
    [Orden]                      INT  NOT NULL,
    [Descripcion] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_dbo.Domicilio] PRIMARY KEY CLUSTERED ([Id] ASC),
);
