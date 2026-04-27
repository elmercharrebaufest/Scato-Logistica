CREATE TABLE [dbo].[DocumentoPorRecorrido]
(
    [Id] INT IDENTITY PRIMARY KEY,
    [Recorrido_Id] INT NOT NULL,
    [Path] NVARCHAR(150) NOT NULL, 
    [Extension] NVARCHAR(10) NOT NULL,
    [Tipo] INT NOT NULL,
    [FechaDeGuardado] DATETIME NOT NULL,

    CONSTRAINT FK_DocumentoPorRecorrido_Recorrido
        FOREIGN KEY (Recorrido_Id)
        REFERENCES Recorrido(Id)
        ON DELETE CASCADE
);
