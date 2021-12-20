export class Mail {
    body: string;
    destinatarios: string[];
    titulo: string;
    adjunto: string | ArrayBuffer;
    nombre: string;

    constructor(titulo: string = null, body : string = null, destinatarios : string[] = null, adjunto : string = null, nombre : string = null){
        this.body = body;
        this.destinatarios = destinatarios;
        this.titulo = titulo;
        this.adjunto = adjunto;
        this.nombre = nombre;
    }
}