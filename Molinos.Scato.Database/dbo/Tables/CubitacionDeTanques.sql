CREATE TABLE [dbo].[CubitacionDeTanques] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [Tk]        NVARCHAR(10) NULL,
    [Altura]    FLOAT NULL,
    [Cantidad]  FLOAT NULL,
    CONSTRAINT [PK_dbo.CubitacionDeTanques] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO