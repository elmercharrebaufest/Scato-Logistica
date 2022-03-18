CREATE TABLE [dbo].[MuestraDeHumedad] (
    [Id]                     INT              IDENTITY (1, 1) NOT NULL,
    [WorkflowInstanceId]     UNIQUEIDENTIFIER NOT NULL,
    [Centro_Id]              INT              NULL,
    [Humedimetro_Id]         INT              NULL,
    [Modalidad]              NVARCHAR (30)    NULL,
    [NumeroOrden]            NVARCHAR (30)    NULL,
    [NumeroDocumentoIngreso] NVARCHAR (30)    NULL,
    [CicloDeCalado]          INT              NULL,
    [NroDeToma]              INT              NULL,
    [ValorLeido]             DECIMAL (18, 2)  NULL,
    [ValorFinal]             DECIMAL (18, 2)  NULL,
    [Fecha]                  DATETIME         NULL,
    [Usuario]                NVARCHAR (30)    NULL,
    [MotivoHumedadManual_Id] INT              NULL,
    [Dispositivo]            NVARCHAR (30)    NULL,
    CONSTRAINT [PK_dbo.MuestraDeHumedad] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.MuestraDeHumedad_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.MuestraDeHumedad_dbo.Humedimetro_Humedimetro_Id] FOREIGN KEY ([Humedimetro_Id]) REFERENCES [dbo].[Humedimetro] ([Id]),
    CONSTRAINT [FK_dbo.MuestraDeHumedad_dbo.Recorrido_MotivoHumedadManual_Id] FOREIGN KEY ([MotivoHumedadManual_Id]) REFERENCES [dbo].[MotivoHumedadManual] ([Id]),
    CONSTRAINT [FK_dbo.MuestraDeHumedad_dbo.Recorrido_Recorrido_Id] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[Recorrido] ([InstanciaWorkflow]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [IX_MuestraDeHumedad_WorkflowInstanceId]
    ON [dbo].[MuestraDeHumedad]([WorkflowInstanceId] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


