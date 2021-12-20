CREATE TABLE [dbo].[GeneradorNumeroHojaDeRuta] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroHojaDeRuta] PRIMARY KEY CLUSTERED ([Id] ASC)
);

