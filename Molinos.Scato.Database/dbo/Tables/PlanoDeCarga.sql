CREATE TABLE [dbo].[PlanoDeCarga] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Observaciones]            NVARCHAR (500)  NULL,
    [CaladoSalida]             DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [FilePathPlano]            VARCHAR (MAX)   NULL,
    [FilePathSecuencia]        VARCHAR (MAX)   NULL,
    [Estiba_Id]                INT             NULL,
    [AgenciaControlPrivado_Id] INT             NULL,
    [Cargado]                  BIT             DEFAULT ((0)) NOT NULL,
    [DefensasMoviles]          BIT             DEFAULT ((0)) NOT NULL,
    [Enviado]                  BIT             DEFAULT ((0)) NOT NULL,
    [Fumigacion]               BIT             DEFAULT ((0)) NOT NULL,
    [EmpresaFumigadora]        NVARCHAR (100)  NULL,
    [FechaDeCreacion]          DATETIME        NULL,
    [FechaDeModificacion]      DATETIME        NULL,
    [Usuario]                  NVARCHAR (40)   NULL,
    [FechaDeFinalizacion]      DATETIME        NULL,
    [UsuarioFinalizacion]      NVARCHAR (40)   NULL,
    CONSTRAINT [PK_dbo.PlanoDeCarga] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.PlanoDeCarga_dbo.AgenciaControlPrivado_AgenciaControlPrivado_Id] FOREIGN KEY ([AgenciaControlPrivado_Id]) REFERENCES [dbo].[AgenciaControlPrivado] ([Id]),
    CONSTRAINT [FK_dbo.PlanoDeCarga_dbo.Estiba_Estiba_Id] FOREIGN KEY ([Estiba_Id]) REFERENCES [dbo].[Estiba] ([Id])
);


