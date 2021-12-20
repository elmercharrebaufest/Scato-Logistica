CREATE TABLE [dbo].[LogExceptuadosTicketMunicipal]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[WorkflowInstanceId]     UNIQUEIDENTIFIER NOT NULL,
	[Motivo]			 NVARCHAR (MAX)   NOT NULL,
	[FechaExcepcion]					DATETIME NOT NULL, 
	[NombreUsuario]			 NVARCHAR (100)   NOT NULL, 
    [NumeroDocumentoIngreso] NVARCHAR(100) NOT NULL, 
    [Patente] NVARCHAR(50) NOT NULL, 
    [FechaIngreso] DATETIME NOT NULL ,
	[Chofer_Id] INT NOT NULL,
	[Material_Id] INT NOT NULL,

	[PagaTicketMunicipal] BIT NOT NULL, 
    CONSTRAINT [FK_dbo.LogExceptuadosTicketMunicipal_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
	CONSTRAINT [FK_dbo.LogExceptuadosTicketMunicipal_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
)
