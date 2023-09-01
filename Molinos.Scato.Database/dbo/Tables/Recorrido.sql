CREATE TABLE [dbo].[Recorrido] (
    [Id]                                INT              IDENTITY (1, 1) NOT NULL,
    [InstanciaWorkflow]                 UNIQUEIDENTIFIER NOT NULL,
    [TipoDocumentoIngreso]              INT              NOT NULL,
    [NumeroDocumentoIngreso]            NVARCHAR (40)    NULL,
    [NumeroDocumentoIngresoLegal]       NVARCHAR (40)    NULL,
    [Patente]                           NVARCHAR (60)    NULL,
    [PesoBruto]                         INT              NULL,
    [PesoTara]                          INT              NULL,
    [PesoBrutoOrigen]                   INT              NULL,
    [PesoTaraOrigen]                    INT              NULL,
    [PesoTaraBodega]                    INT              NULL,
    [PesoNetoBodegaEnLitros]            INT              NULL,
    [Workflow_Id]                       INT              NOT NULL,
    [Centro_Id]                         INT              NOT NULL,
    [Chofer_Id]                         INT              NULL,
    [Transportista_Id]                  INT              NULL,
    [TipoComercial_Id]                  INT              NOT NULL,
    [Material_Id]                       INT              NULL,
    [DatosProximaActividad]             NVARCHAR (30)    NULL,
    [Almacen_Id]                        INT              NULL,
    [Calado_Id]                         INT              NULL,
    [AnalisisDeCalidad_Id]              INT              NULL,
    [PesoBrutoFecha]                    DATETIME         NULL,
    [PesoTaraFecha]                     DATETIME         NULL,
    [PesoBrutoUsuario]                  NVARCHAR (30)    NULL,
    [PesoTaraUsuario]                   NVARCHAR (30)    NULL,
    [BalanzaTara_Id]                    INT              NULL,
    [BalanzaBruto_Id]                   INT              NULL,
    [FechaInicio]                       DATETIME         NULL,
    [Rechazado]                         BIT              DEFAULT ((0)) NOT NULL,
    [Usuario]                           NVARCHAR (30)    NULL,
    [Vehiculo_Id]                       INT              NULL,
    [WorkflowDefinicion_Id]             INT              NOT NULL,
    [FechaEgreso]                       DATETIME         NULL,
    [DocumentoInternoSap]               NVARCHAR (40)    NULL,
    [NumeroDeDocumentoSap]              NVARCHAR (40)    NULL,
    [NumeroCot]                         NVARCHAR (40)    NULL,
    [NumeroCiu]                         NVARCHAR (8)     NULL,
    [PesoNetoTransile]                  INT              NULL,
    [Terminado]                         BIT              DEFAULT ((0)) NOT NULL,
    [Calle_Id]                          INT              NULL,
    [ControlBalanza]                    BIT              DEFAULT ((0)) NOT NULL,
    [TarjetaDeAcceso]                   NVARCHAR (10)    NULL,
    [EnvioMuestraAuditoria]             BIT              DEFAULT ((0)) NOT NULL,
    [TipoDocumentoIngresoRelacionado]   INT              NULL,
    [NumeroDocumentoIngresoRelacionado] NVARCHAR (40)    NULL,
    [Establecimiento_Id]                INT              NULL,
    [PagaTicketMunicipal]               BIT              NULL,
    [TipoVehiculo]                      INT              DEFAULT ((0)) NOT NULL,
    [DescuentoEnKgOncca]                DECIMAL (18, 2)  DEFAULT ((0)) NOT NULL,
    [PesoBrutoModalidad]                INT              NULL,
    [PesoTaraModalidad]                 INT              NULL,
    [CaladoEnPlanta_Id]                 INT              NULL,
    [CorrespondeCaladoEnPlanta]         BIT              DEFAULT ((0)) NOT NULL,
    [EnvioMuestraAuditoriaCamara]       BIT              DEFAULT ((0)) NOT NULL,
    [VehiculoDemorado]                  BIT              DEFAULT ((0)) NOT NULL,
    [EstablecimientoDemorado]           BIT              DEFAULT ((0)) NOT NULL,
    [MotivoDemora]                      NVARCHAR (1000)  NULL,
    [SacoTurnoConCircular]              BIT              DEFAULT ((0)) NOT NULL,
    [LlegoEnHorario]                    BIT              DEFAULT ((0)) NOT NULL,
    [PasoPorContingenciaPesoExc] BIT DEFAULT ((0)) NOT NULL, 
    [PuntoDeCarga_Id] INT NULL,
    CONSTRAINT [PK_dbo.Recorrido] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Almacen_Almacen_Id] FOREIGN KEY ([Almacen_Id]) REFERENCES [dbo].[Almacen] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.AnalisisDeCalidad_AnalisisDeCalidad_Id] FOREIGN KEY ([AnalisisDeCalidad_Id]) REFERENCES [dbo].[AnalisisDeCalidad] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.BalanzaBruto_Balanza_Id] FOREIGN KEY ([BalanzaBruto_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.BalanzaTara_Balanza_Id] FOREIGN KEY ([BalanzaTara_Id]) REFERENCES [dbo].[Balanza] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Calado_Calado_Id] FOREIGN KEY ([Calado_Id]) REFERENCES [dbo].[Calado] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.CaladoEnPlanta_CaladoEnPlanta_Id] FOREIGN KEY ([CaladoEnPlanta_Id]) REFERENCES [dbo].[CaladoEnPlanta] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Calle_Calle_Id] FOREIGN KEY ([Calle_Id]) REFERENCES [dbo].[Calle] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Establecimiento_Establecimiento_Id] FOREIGN KEY ([Establecimiento_Id]) REFERENCES [dbo].[Establecimiento] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.TipoDocumentoIngreso_TipoDocumentoIngreso_Id] FOREIGN KEY ([TipoDocumentoIngreso]) REFERENCES [dbo].[TipoDocumentoIngreso] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Transportista_Transportista_Id] FOREIGN KEY ([Transportista_Id]) REFERENCES [dbo].[Transportista] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Vehiculo_Vehiculo_Id] FOREIGN KEY ([Vehiculo_Id]) REFERENCES [dbo].[Vehiculo] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.Workflow_Workflow_Id] FOREIGN KEY ([Workflow_Id]) REFERENCES [dbo].[Workflow] ([Id]),
    CONSTRAINT [FK_dbo.Recorrido_dbo.WorkflowDefinicion_WorkflowDefinicion_Id] FOREIGN KEY ([WorkflowDefinicion_Id]) REFERENCES [dbo].[WorkflowDefinicion] ([Id]),
    CONSTRAINT [UK_Recorrido_InstanciaWorkflow] UNIQUE NONCLUSTERED ([InstanciaWorkflow] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_Recorrido_PuntoDeCarga] FOREIGN KEY ([PuntoDeCarga_Id]) REFERENCES [dbo].[PuntoDeCarga] ([Id])
);



GO 

CREATE NONCLUSTERED INDEX [IX_FechaInicio_Centro_Id]
    ON [dbo].[Recorrido]([FechaInicio] ASC, [Centro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Workflow_Id]
    ON [dbo].[Recorrido]([Workflow_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[Recorrido]([Material_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Recorrido]([Centro_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Almacen_Id]
    ON [dbo].[Recorrido]([Almacen_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Chofer_Id]
    ON [dbo].[Recorrido]([Chofer_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Transportista_Id]
    ON [dbo].[Recorrido]([Transportista_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_TipoComercial_Id]
    ON [dbo].[Recorrido]([TipoComercial_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [IX_Calado_Id]
    ON [dbo].[Recorrido]([Calado_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_AnalisisDeCalidad_Id]
    ON [dbo].[Recorrido]([AnalisisDeCalidad_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



GO
CREATE NONCLUSTERED INDEX [IX_Recorrido_Centro_Id_FechaInicio]
    ON [dbo].[Recorrido]([Centro_Id] ASC, [FechaInicio] ASC)
    INCLUDE([Material_Id], [Calado_Id], [AnalisisDeCalidad_Id], [Vehiculo_Id]) WITH (STATISTICS_NORECOMPUTE = ON);


GO

CREATE NONCLUSTERED INDEX [IX_Recorrido_TarjetaDeAcceso_Terminado]
    ON [dbo].[Recorrido]([TarjetaDeAcceso] ASC, [Terminado] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);



GO 

CREATE NONCLUSTERED INDEX [IX_Centro_id_Terminado_incl_varios]
    ON [dbo].[Recorrido] ([Centro_Id],[Terminado]) 
    INCLUDE ([InstanciaWorkflow],[TipoDocumentoIngreso],[NumeroDocumentoIngreso],[Patente],[Workflow_Id],[Chofer_Id],[Transportista_Id],
        [TipoComercial_Id],[Material_Id],[Almacen_Id],[Calado_Id],[FechaInicio],[Rechazado],[TarjetaDeAcceso],[Establecimiento_Id],[PagaTicketMunicipal],
        [TipoVehiculo])



GO


CREATE TRIGGER [DELETE_Recorrido]
   ON dbo.[Recorrido]
   INSTEAD OF DELETE
AS 
BEGIN
 SET NOCOUNT ON;
 DELETE FROM Impresion WHERE Impresion.[WorkflowId] IN (SELECT [InstanciaWorkflow] FROM DELETED)
 DELETE FROM TransmisionASap WHERE TransmisionASap.InstanciaWorkflow IN (SELECT [InstanciaWorkflow] FROM DELETED)
 DELETE FROM Recorrido WHERE Recorrido.Id IN (SELECT Id FROM DELETED)
END
GO
CREATE NONCLUSTERED INDEX [ndx_InstanciaWorkflow_numeroDocumentoIngreso_Patente]
    ON [dbo].[Recorrido]([InstanciaWorkflow] ASC, [NumeroDocumentoIngreso] ASC, [Patente] ASC, [TipoDocumentoIngreso] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_WorkflowId_Centro_id_Material_id_Rechazado_PesoTaraFecha]
    ON [dbo].[Recorrido]([Workflow_Id] ASC, [Material_Id] ASC, [Centro_Id] ASC, [Rechazado] ASC, [PesoTaraFecha] ASC) WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);

