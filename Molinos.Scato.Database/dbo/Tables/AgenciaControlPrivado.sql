CREATE TABLE [dbo].[AgenciaControlPrivado] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]					 NVARCHAR(50)             NOT NULL,
    CONSTRAINT [PK_dbo.AgenciaControlPrivado] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_AgenciaControlPrivado_Nombre] UNIQUE (Nombre)
);
