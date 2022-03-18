CREATE TABLE [dbo].[MotivosFallasBalanza] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,
    [Siglas] NVARCHAR (60) NOT NULL,
    CONSTRAINT [PK_dbo.MotivosFallasBalanza] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);

