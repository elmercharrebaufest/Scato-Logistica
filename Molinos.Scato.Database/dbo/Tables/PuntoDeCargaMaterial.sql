CREATE TABLE PuntoDeCargaMaterial (
    Material_Id INT,
    PuntoDeCarga_Id INT,
    PRIMARY KEY (Material_Id, PuntoDeCarga_Id),
    FOREIGN KEY (Material_Id) REFERENCES Material(Id),
    FOREIGN KEY (PuntoDeCarga_Id) REFERENCES PuntoDeCarga(Id)
);