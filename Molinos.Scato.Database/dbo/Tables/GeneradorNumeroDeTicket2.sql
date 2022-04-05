CREATE TABLE [dbo].[GeneradorNumeroDeTicket2] (
    [PuestoDeTrabajo_Id] INT NOT NULL,
    [Id]                 INT NOT NULL,
    [PagoConMercadoPago] BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_GeneradorNumeroDeTicket2] PRIMARY KEY CLUSTERED ([PuestoDeTrabajo_Id] ASC, [Id] ASC, [PagoConMercadoPago] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.GeneradorNumeroDeTicket2_dbo.PuestoDeTrabajo_PuestoDeTrabajo_Id] FOREIGN KEY ([PuestoDeTrabajo_Id]) REFERENCES [dbo].[PuestoDeTrabajo] ([Id])
);



GO


GO


GO

CREATE TRIGGER InsteadTriggerGeneradorNumeroDeTicket2 on GeneradorNumeroDeTicket2 INSTEAD OF INSERT AS
BEGIN
	BEGIN TRANSACTION

		DECLARE @PK INT

		SELECT @PK = ISNULL(MAX([Id]),0) + 1
		FROM GeneradorNumeroDeTicket2
		WHERE [PuestoDeTrabajo_Id] = (SELECT [PuestoDeTrabajo_Id] FROM inserted)
		AND PagoConMercadoPago= (SELECT PagoConMercadoPago FROM inserted)

		INSERT INTO GeneradorNumeroDeTicket2 ([Id],[PuestoDeTrabajo_Id],PagoConMercadoPago)
		SELECT @PK,[PuestoDeTrabajo_Id],PagoConMercadoPago

		FROM inserted

	COMMIT TRANSACTION

END
GO
