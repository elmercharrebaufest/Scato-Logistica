CREATE TABLE [dbo].[ConfiguracionMail]
(
	[Id] INT NOT NULL PRIMARY KEY identity(1,1), 
    [TemplateMail] VARCHAR(100) NOT NULL, 
    [Direcciones] VARCHAR(MAX) NOT NULL
)
