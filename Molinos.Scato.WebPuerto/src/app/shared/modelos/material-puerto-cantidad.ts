export class MaterialPuertoCantidad{
    materialId : number;
    descripcionCorta: string;
    cantidad : number;
    esLiquido: boolean;
    color: string;
    public constructor(init?:Partial<MaterialPuertoCantidad>) {
        Object.assign(this, init);
    }
}