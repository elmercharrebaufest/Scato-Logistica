CREATE TABLE [dbo].[MotivoForzarCero] (
    [Id]                        INT				IDENTITY (1, 1) NOT NULL,
    [Balanza_Id]          INT				NOT NULL,
    [Fecha]						DATETIME		NOT NULL, 
    [Motivo]					NVARCHAR(MAX)	NOT NULL, 
    [InstanceId] UNIQUEIDENTIFIER NOT NULL, 
    [Usuario] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_dbo.MotivoForzarCero] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.MotivoForzarCero_dbo.MotivoForzarCero_Balanza_Id] FOREIGN KEY ([Balanza_Id]) REFERENCES [dbo].[Balanza] ([Id])
);
