CREATE TABLE [dbo].[LlamadoAutomaticoHidraulica]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Estado] INT NOT NULL DEFAULT 0, 
    [Hidraulica_Id] INT NOT NULL UNIQUE, 
    [UltimaPatenteLlamada] NVARCHAR(10) NULL, 
    [FechaUltimaModificacionEstado] DATETIME NULL,
    [UltimoCartelLlamado] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_dbo.LlamadoAutomaticoHidraulica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.LlamadoAutomaticoHidraulica_dbo.PuestosDeCargaDescarga_PuestosDeCargaDescarga_Id] FOREIGN KEY ([Hidraulica_Id]) REFERENCES [dbo].[PuestosDeCargaDescarga] ([Id])
)
