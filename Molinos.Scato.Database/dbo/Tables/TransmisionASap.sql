CREATE TABLE [dbo].[TransmisionASap] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [InstanciaWorkflow] UNIQUEIDENTIFIER NOT NULL,
    [Estado]            INT              NOT NULL,
    [Fecha]             DATETIME         NOT NULL,
    [FuncionSap]        INT              NOT NULL,
    [MensajeError]      NVARCHAR (255)   NULL,
    CONSTRAINT [PK_dbo.TransmisionASap] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.TransmisionASap_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([InstanciaWorkflow]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow])
);



GO

CREATE NONCLUSTERED INDEX [IX_TransmisionASap_InstanciaWorkflow]
    ON [dbo].[TransmisionASap]([InstanciaWorkflow] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


