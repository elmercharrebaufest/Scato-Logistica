CREATE TABLE RegistroInactividad (
    Id int NOT NULL IDENTITY(1,1),
    Usuario varchar(100) NOT NULL,
	MotivoId int NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (MotivoId) REFERENCES MotivoInactividad(Id),
    FechaInicio datetime NOT NULL,
	FechaFinal datetime NULL,
    PuestoTrabajoId int NULL
);
