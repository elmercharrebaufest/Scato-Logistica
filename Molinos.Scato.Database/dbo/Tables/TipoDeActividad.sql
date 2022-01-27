CREATE TABLE [dbo].[TipoDeActividad] (
    [Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Codigo] NCHAR(5), 
    [Descripcion] NVARCHAR(255)
);