CREATE TABLE [dbo].CallePreBalanzaPlayaInterna (
    [Id]                      INT   IDENTITY (1, 1) NOT NULL,
    [CallePlayaInterna_Id]     INT   NOT NULL,
    [CallePreBalanza_Id]       INT   NOT NULL,
    [FechaLlamado]  DATETIME   NOT NULL,
    [Recorrido_Id] INT NULL, 
    [CodigoAutomatismoTipoLlamado] NVARCHAR(10) NULL, 
    [EsCamionEnEspera] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.CallePreBalanzaPlayaInterna] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CallePreBalanzaPlayaInterna_dbo.CallePlayaInterna_Calle_Id] FOREIGN KEY (CallePlayaInterna_Id) REFERENCES [dbo].Calle ([Id]),
    CONSTRAINT [FK_dbo.CallePreBalanzaPlayaInterna_dbo.CallePreBalanza_Calle_Id] FOREIGN KEY (CallePreBalanza_Id) REFERENCES [dbo].Calle ([Id]),
    CONSTRAINT [FK_dbo.CallePreBalanzaPlayaInterna_dbo.Recorrido_Recorrido_Id] FOREIGN KEY (Recorrido_Id) REFERENCES [dbo].Recorrido ([Id]),
);




