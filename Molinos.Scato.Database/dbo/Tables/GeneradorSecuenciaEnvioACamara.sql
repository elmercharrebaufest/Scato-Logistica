CREATE TABLE [dbo].[GeneradorSecuenciaEnvioACamara] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorSecuenciaEnvioACamara] PRIMARY KEY CLUSTERED ([Id] ASC)
);

