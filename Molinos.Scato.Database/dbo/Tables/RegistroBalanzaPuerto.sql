CREATE TABLE [dbo].[RegistroBalanzaPuerto] (
    [Id]            INT           NOT NULL,
    [NumeroBalanza] NVARCHAR (50) NOT NULL,
    [Tipo]          NVARCHAR (30) NOT NULL,
    [Fecha]         DATETIME      NOT NULL,
    [EnviadoASap]   BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.RegistroBalanzaPuerto] PRIMARY KEY CLUSTERED ([Id] ASC, [NumeroBalanza] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON)
);


GO

CREATE NONCLUSTERED INDEX [IX_RegistroBalanzaPuerto_Tipo_Fecha]
    ON [dbo].[RegistroBalanzaPuerto]([Tipo] ASC, [Fecha] ASC)
    INCLUDE([Id], [NumeroBalanza]) WITH (STATISTICS_NORECOMPUTE = ON);


GO