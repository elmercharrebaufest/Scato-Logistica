UPDATE PuestoDeTrabajo 
SET EncolaLecturas = 0
WHERE PidePatente = 0

UPDATE PuestoDeTrabajo 
SET EncolaLecturas = 1
WHERE PidePatente = 1