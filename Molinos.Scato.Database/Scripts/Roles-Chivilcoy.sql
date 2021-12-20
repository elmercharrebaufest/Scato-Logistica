-- Roles 

INSERT INTO Rol (Descripcion) VALUES ('Receptor/Balancero (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Coordinador de Playa (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Perito (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Coordinador (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Entregador (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Usuario Experto (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Jefe Admin. (Chivilcoy)');
INSERT INTO Rol (Descripcion) VALUES ('Auditor (Chivilcoy)');

-- Asignación de permisos por rol

-- Receptor / Balancero
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Receptor/Balancero (Chivilcoy)' AND p.Codigo in
(101,102,104,105,106,108,109,110,111,112,113,114,115,116,117,118,120,121,122,123,124,125,126,127,128,129,130,132,133,134,135,136,137,138,139,140,141,200,300)

-- Coordinador de playa
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Coordinador de Playa (Chivilcoy)' AND p.Codigo in (101, 111, 118, 132)

-- Perito
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Perito (Chivilcoy)' AND p.Codigo in (103, 111, 119)

-- Coordinador
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Coordinador (Chivilcoy)' AND p.Codigo in (103, 107, 111, 119, 201, 202, 207, 208)

-- Entregador
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Entregador (Chivilcoy)' AND p.Codigo in (100, 111)

-- Jefe Admin.
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Jefe Admin. (Chivilcoy)' AND p.Codigo in
(2,3,4,5,6,7,8,12,13,17,20,22,23,24,26,29,30,31,33,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,132,133,134,135,136,137,138,139,140,141,200,201,202,203,204,205,206,207,208,209,210,300,301,400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,420,421,422)

-- Auditor
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Auditor (Chivilcoy)' AND p.Codigo in
(400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,417,418,419,420,421,422)


-- Usuario Experto
insert into RolPermiso(Rol_Id, Permiso_Id) 
select r.id, p.id 
from Rol r, Permiso p
where r.Descripcion = 'Usuario Experto (Chivilcoy)' and p.Id not in (select rp.Permiso_Id from RolPermiso rp where rp.Rol_Id = r.Id)
