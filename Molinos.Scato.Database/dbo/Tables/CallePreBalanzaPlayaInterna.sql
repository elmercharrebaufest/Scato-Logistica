CREATE TABLE [dbo].CallePreBalanzaPlayaInterna (
    [Id]                      INT   IDENTITY (1, 1) NOT NULL,
    [CallePlayaInterna_Id]     INT   NOT NULL,
    [CallePreBalanza_Id]       INT   NOT NULL,
    [FechaLlamado]  DATETIME   NOT NULL,
    CONSTRAINT [PK_dbo.CallePreBalanzaPlayaInterna] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.CallePreBalanzaPlayaInterna_dbo.CallePlayaInterna_Calle_Id] FOREIGN KEY (CallePlayaInterna_Id) REFERENCES [dbo].Calle ([Id]),
    CONSTRAINT [FK_dbo.CallePreBalanzaPlayaInterna_dbo.CallePreBalanza_Calle_Id] FOREIGN KEY (CallePreBalanza_Id) REFERENCES [dbo].Calle ([Id]),
);




