CREATE TABLE [dbo].[KmPorProveedor] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Centro_Id]                  INT NOT NULL,
    [Cliente_Id]                 INT NOT NULL,
	[KmARecorrer]				NVARCHAR(10) NOT NULL,
	[Localidad_Id]				INT NOT NULL,
    CONSTRAINT [PK_dbo.KmPorProveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
);

