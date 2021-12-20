#Generar nuevos componentes

Existen 2 categorias de componentes, si el componente se comparte casi en su totalidad en 2 o mas componentes va en la ruta: /src/app/shared/componentes/{carpeta de destino o /modulos}. Y agregarlo en la constante componentes.
Si el componente es propio de un modulo, se agrega en la ruta: /src/app/modulos/{carpeta de destino}

Para generar el componente ir a la carpeta destino y ejecutar el comando: ng g c <nombre-del-componente> --skip-tests o ng generate component <nombre-del-componente> --skip-tests

#Routing del proyecto

El proyecto se carga por lazy loading, por lo que cada subruta nueva debe agregarse en el routing padre con el siguiente objeto:
{
    path: 'ruta',
    loadchildren: () => import('ruta relativa del modulo hijo').then(m => m.nombredelmodulo)
}

Para el modulo hijo que seria el que va a cargar esa nueva subruta solo debe importar 3 modulos: 

> <nombredelmodulorouting>
> CommonModule
> SharedModule

Y debe declarar los componentes que van a incluirse en esa nueva ruta (Nota: no declarar los componentes en shared, ya que van en el SharedModule).

#Librerias
Instalar la libreria en la raiz del proyecto a traves de npm install (Nota: Revistar si la libreria es para solo desarrollo o produccion, en caso de ser solo desarrollo debe llevar el flag --save-dev)

Para agregar librerias hacerlo en la constante 'libs' en la ruta /src/app/shared/components/shared-components.module.ts


#Guards y Navegacion

##Auth

-Para la comprobacion de auth en rutas, se utiliza el guard: 
> src/shared/seguridad/login.guard.ts

-Para la autenticacion de usuarios, se utiliza el servicio: 
> src/shared/servicios/autenticador.service.ts

-Para el manejo de data de usuario, se utiliza el servicio:
> src/shared/servicios/session.service.ts
Desde esta ruta, se puede obtener username y permisos

-Para el manejo de permisos y activacion de rutas, se utiliza el guard:
> src/shared/seguridad/role.guard.ts

-Para la gestion de permisos utilizamos el enum:
> src/shared/enums/permisos-scato.ts
