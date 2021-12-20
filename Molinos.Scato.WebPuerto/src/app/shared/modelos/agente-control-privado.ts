
export class AgenteControlPrivado {
      id : number; 
      nombre: string;
      apellido: string;
      name: string; 

      constructor(id, nombre, apellido){
            this.id = id;
            this.nombre = nombre;
            this.apellido = apellido;
            this.name = this.nombre + ' ' + this.apellido;
      }
}
