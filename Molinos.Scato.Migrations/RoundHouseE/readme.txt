CARPETAS

01 AlterDatabase   				
02 RunAfterCreateDatabase		
03 RunBeforeUp					
04 Up							
05 RunFirstAfterUp				
06 Functions					
07 Views						
08 StoredProcedures		
09 Triggers	
10 Indexes		
11 RunAfterOtherAnyTimeScripts	
12 Permissions

USO DE LAS CARPETAS

01  Scripts que alteren a la DB
02  Scripts que se deban correr luego de crear la DB (solo sirve si RoundHousE creo la DB)
03 	Scripts que se desean corran antes de las actualizaciones
04 	Scripts de actualizacion de tablas , etc
05 	Scripts que se desean corran luego de las actualizaciones
06 	Scripts de funciones
07 	Scripts de vistas
08 	Scripts de stores
09 	Scripts de triggers
10 	Scripts de indexes
11 	Scripts que se quiere se ejecuten luego de todos los procesos
12 	Scripts para permisos de DB

TIPOS DE EJECUCION

01  Se ejecuta siempre que existan cambios en el script				
02  Se ejecuta solo una vez , si se cambia arrojara una advertencia
03 	Se ejecuta siempre que existan cambios en el script
04 	Se ejecuta solo una vez , si se cambia arrojara una advertencia
05 	Se ejecuta siempre que existan cambios en el script
06 	Se ejecuta siempre que existan cambios en el script
07 	Se ejecuta siempre que existan cambios en el script
08 	Se ejecuta siempre que existan cambios en el script
09 	Se ejecuta siempre que existan cambios en el script
10 	Se ejecuta siempre que existan cambios en el script
11 	Se ejecuta siempre que existan cambios en el script
12 	Se ejecuta siempre que se deploye