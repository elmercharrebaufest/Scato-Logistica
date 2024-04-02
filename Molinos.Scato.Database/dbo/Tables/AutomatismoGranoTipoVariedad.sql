CREATE TABLE AutomatismoGranoTipoVariedad (
    AutomatismoGrano_Id INT,
    TipoVariedad_Id INT,
    CONSTRAINT PK_AutomatismoGranoTipoVariedad PRIMARY KEY (AutomatismoGrano_Id, TipoVariedad_Id),
    CONSTRAINT FK_AutomatismoGrano FOREIGN KEY (AutomatismoGrano_Id) REFERENCES AutomatismoGrano(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TipoVariedad FOREIGN KEY (TipoVariedad_Id) REFERENCES TipoVariedad(Id)
);
