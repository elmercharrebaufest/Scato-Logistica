CREATE TABLE [dbo].[AutomatismoNoGranoPuntoDeCarga] (
    [AutomatismoNoGrano_Id]     INT NOT NULL,
    [PuntoDeCarga_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.AutomatismoNoGranoPuntoDeCarga] PRIMARY KEY CLUSTERED ([AutomatismoNoGrano_Id] ASC, [PuntoDeCarga_Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AutomatismoNoGranoPuntoDeCarga_dbo.PuntoDeCarga_PuntoDeCarga_Id] FOREIGN KEY ([PuntoDeCarga_Id]) REFERENCES [dbo].[PuntoDeCarga] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AutomatismoNoGranoPuntoDeCarga_dbo.AutomatismoNoGrano_AutomatismoNoGrano_Id] FOREIGN KEY ([AutomatismoNoGrano_Id]) REFERENCES [dbo].[AutomatismoNoGrano] ([Id]) ON DELETE CASCADE
);