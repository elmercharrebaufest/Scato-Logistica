CREATE TABLE [dbo].[LlegadaAdestinosEnRedespachosTransmisionASap] (
    [Id]           INT            NOT NULL,
	[Documento]  NVARCHAR (100) NULL,
	[Ejercicio]  NVARCHAR (100) NULL,
	[FechaContab]  NVARCHAR (100) NULL,
	[DocLegal]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.LlegadaAdestinosEnRedespachosTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.LlegadaAdestinosEnRedespachosTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO