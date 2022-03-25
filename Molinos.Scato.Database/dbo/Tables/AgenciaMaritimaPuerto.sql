CREATE TABLE [dbo].[AgenciaMaritimaPuerto] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (60) NOT NULL,
    CONSTRAINT [PK_dbo.AgenciaMaritimaPuerto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_AgenciaMaritimaPuerto_Nombre] UNIQUE NONCLUSTERED ([Nombre] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


GO