CREATE TABLE [dbo].[TipoEmbalaje]
(
	[Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]      CHAR (3)       NOT NULL,
    [Descripcion] NVARCHAR (256) NULL,
    [Activo]      BIT            NOT NULL,
    CONSTRAINT [PK_TipoEmbalaje] PRIMARY KEY CLUSTERED ([Id] ASC)
)
