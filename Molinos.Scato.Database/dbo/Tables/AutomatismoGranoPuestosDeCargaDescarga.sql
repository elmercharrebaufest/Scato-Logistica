CREATE TABLE [dbo].[AutomatismoGranoPuestosDeCargaDescarga] (

  [AutomatismoGrano_Id] INT NOT NULL,

  [PuestosDeCargaDescarga_Id] INT NOT NULL,

  CONSTRAINT [PK_AutomatismoGranoPuestosDeCargaDescarga_Id] PRIMARY KEY CLUSTERED ([AutomatismoGrano_Id] ASC, [PuestosDeCargaDescarga_Id] ASC),

  CONSTRAINT [FK_AutomatismoGranoPuestosDeCargaDescarga_AutomatismoGrano_Id] 
    FOREIGN KEY([AutomatismoGrano_Id]) REFERENCES [dbo].[AutomatismoGrano]([Id]) ON DELETE CASCADE,

  CONSTRAINT [FK_AutomatismoGranoPuestosDeCargaDescarga_PuestosDeCargaDescarga_Id]
    FOREIGN KEY([PuestosDeCargaDescarga_Id]) REFERENCES [dbo].[PuestosDeCargaDescarga]([Id])

)
