CREATE TABLE [dbo].[MotivoHumedadManual] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (40) NOT NULL,    
    CONSTRAINT [PK_dbo.MotivoHumedadManual] PRIMARY KEY CLUSTERED ([Id] ASC)
);

