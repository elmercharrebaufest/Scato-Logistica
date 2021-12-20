CREATE TABLE [dbo].[RegistroStockEPA] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
	[CodigoEstablecimiento]         NVARCHAR(10) NOT NULL,
    [Cosecha]         NVARCHAR (10) NOT NULL,
	[Recorrido_Id]  INT NOT NULL,
    [PesoNeto]    DECIMAL(18, 2) NOT NULL,
    [EPApesoDescontadoTildado] BIT NOT NULL, 
    CONSTRAINT [PK_dbo.RegistroStockEPA] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.RegistroStockEPA_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([Recorrido_Id]) REFERENCES [dbo].Recorrido ([Id]) ON DELETE CASCADE
);