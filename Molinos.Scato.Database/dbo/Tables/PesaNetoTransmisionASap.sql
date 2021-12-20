CREATE TABLE [dbo].[PesaNetoTransmisionASap] (
    [Id]           INT            NOT NULL,
	[PesoBruto]  DECIMAL(18, 2) NULL,
	[PesoNeto]  DECIMAL(18, 2) NULL,
	[NumeroDocumento]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.PesaNetoTransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.PesaNetoTransmisionASap.TransmisionASapId] FOREIGN KEY ([Id]) REFERENCES [dbo].[TransmisionASap] ([Id]) ON DELETE CASCADE
);

GO