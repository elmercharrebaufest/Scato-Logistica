CREATE TABLE [dbo].[ConfiguracionMail] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [TemplateMail] VARCHAR (100) NOT NULL,
    [Direcciones]  VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);


