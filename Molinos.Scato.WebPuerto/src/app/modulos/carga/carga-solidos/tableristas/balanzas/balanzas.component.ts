import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
// Excel
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';

import { Tipoalerta } from '@ScatoEnums/tipo-alerta';
// Models
import { Balanza } from '@ScatoModels/balanza';
import { Balanza78 } from '@ScatoModels/balanza78';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';
import { MotivosFallasBalanza } from '@ScatoModels/motivo-balanza';
// Services
import { Balanzas78Service } from '@ScatoServicios/balanzas78.service';
import { ConfirmationDialogService } from '@ScatoServicios/confirmation-dialog.service';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { EmbarqueService } from '@ScatoServicios/embarque.service';
import { ModuloDeCargaService } from '@ScatoServicios/modulo-de-carga.service';
import { WorkflowService } from '@ScatoServicios/workflow.service';
import { BalanzaService } from '@ScatoServicios/balanza.service';

interface TotToneladas {
  producto: string;
  toneladas: number;
}

@Component({
  selector: 'app-balanzas',
  templateUrl: './balanzas.component.html',
  styleUrls: ['./balanzas.component.css']
})
export class BalanzasComponent implements OnInit {
  errorMessage: boolean = false;
  formInitialValues: any;
  balanza7Form: FormGroup;
  balanza8Form: FormGroup;
  motivosBalanzas78: MotivosFallasBalanza[];
  listadoBalanza7: Balanza78[] = [];
  listadoBalanza8: Balanza78[] = [];
  resultado7: TotToneladas[] = [];
  resultado8: TotToneladas[] = [];
  seleccionado: boolean = false;
  embarqueId: number;
  vaporId: number = 0;
  embarque: EmbarqueNav;
  nombreBuqueEsDiferente: boolean = false;
  balanza7DiferenteNombre: boolean = false;
  balanza8DiferenteNombre: boolean = false;
  balanzasIncompletas: boolean = false;
  valorAleatorio: number = 0;
  barquitos: InstanciaWorkflowPuerto[] = [];
  balanzadasEnForm = [];
  balanzadasFueraDeForm = [];
  materialesPuerto = [];

  fileName = 'Balanzas_7y8.xlsx';

  confirmationDialogService: any;

  constructor(private _modalService: NgbModal,
    private formBuilder: FormBuilder,
    private _procesoService: DatosEmbarquesProcesoService,
    private moduloDeCargaService: ModuloDeCargaService,
    private balanzas78Service: Balanzas78Service,
    config: NgbModalConfig,
    confirmationDialogService: ConfirmationDialogService,
    private embarqueService: EmbarqueService,
    private workflowService: WorkflowService,
    private balanzaService: BalanzaService) {
    // customize default values of modals used by this component tree
    config.backdrop = 'static';
    config.keyboard = false;

    this.confirmationDialogService = confirmationDialogService;

    this.embarque = this._procesoService.getEmbarqueSelected();
    this.embarqueId = this._procesoService.getEmbarqueId();

    this.embarqueService.obtenerListadoMateriales().subscribe( mat => this.materialesPuerto = mat );

    this.workflowService.obtenerListado().subscribe((resp: any) => {
      this.barquitos = resp.find(x => x.embarque.id === this.embarqueId);
      this.vaporId = this.barquitos['embarque'].vapor.id;
    });

    this.cargarMotivosBalanzas78();
  }

  ngOnInit(): void {

    this.balanza7Form = this.formBuilder.group({
      balanzas7: this.formBuilder.array([this.initBalanzas7()])
    });

    this.balanza8Form = this.formBuilder.group({
      balanzas8: this.formBuilder.array([this.initBalanzas8()])
    });

    this.formInitialValues = this.balanza7Form.getRawValue();

    // this.obtenerBalanzadas7();
    this.obtenerBalanzadasEnVivo();
  }

  cargarMotivosBalanzas78() {
    this.moduloDeCargaService.obtenerListadoMotivosFallasBalanza()
      .subscribe((motivos: any) => {
        this.motivosBalanzas78 = motivos;
      });
  }

  initBalanzas7(x: Balanza78 = null) {
    return this.formBuilder.group({
      id: x?.id ?? 0,
      numeroBalanza: x?.numeroBalanza ?? "",
      fecha: x?.fecha ?? "",
      hora: x?.hora ?? "",
      toneladas: x?.toneladas ?? "",
      producto: x?.producto ?? "",
      bodega: x?.bodega ?? "",
      porcentajeCarga: x?.porcentajeCarga ?? "",
      totalProducto: x?.totalProducto ?? "",
      motivosFallasBalanza: x?.motivosFallasBalanza ?? "",
      observaciones: x?.observaciones ?? "",
      seleccionado: false,
      nombreBuque: x?.nombreBuque ?? "",
    });
  }

  initBalanzas8(x: Balanza78 = null) {
    return this.formBuilder.group({
      id: x?.id ?? 0,
      numeroBalanza: x?.numeroBalanza ?? "",
      fecha: x?.fecha ?? "",
      hora: x?.hora ?? "",
      toneladas: x?.toneladas ?? "",
      producto: x?.producto ?? "",
      bodega: x?.bodega ?? "",
      porcentajeCarga: x?.porcentajeCarga ?? "",
      totalProducto: x?.totalProducto ?? "",
      motivosFallasBalanza: x?.motivosFallasBalanza ?? "",
      observaciones: x?.observaciones ?? "",
      seleccionado: false,
      nombreBuque: x?.nombreBuque ?? "",
    });
  }

  obtenerBalanzadasEnVivo() {
    setInterval(() => {
      this.balanzaService.listarBalanzadaBuque(this.vaporId)
        .subscribe(resp => {
          let balanzadasArray = this.convertirBalanzadas(resp);
          let filtroBalanza7 = balanzadasArray.filter(x => x.numeroBalanza == 7);
          let filtroBalanza8 = balanzadasArray.filter(x => x.numeroBalanza == 8);

          this.actualizarBalanzadas7(filtroBalanza7);
          this.actualizarBalanzadas8(filtroBalanza8);
        })
    }, 20000);
  }

  convertirBalanzadas(bal: Balanza[]): Balanza78[] {
    let balanzas = [];

    for (let b of bal) {
      let lFecha = b.fecha.toString().substr(0, 10);
      let lHora = b.fecha.toString().substr(11, 5);
      let {descripcionCorta} = this.materialesPuerto.find( x => x.id = b.cargaInicial.materialId );

      let propiedades = {
        "id": b.id,
        "nombreBuque": b.cargaInicial.vapor,
        "numeroBalanza": b.numeroBalanza,
        "fecha": lFecha,
        "hora": lHora,
        "toneladas": b.pesoNeto,
        "producto": descripcionCorta,
        "bodega": b.cargaInicial.bodega,
        "porcentajeCarga": b.pesoNeto * 100 / b.cargaInicial.pesoProgramado,
        "totalProducto": 0,
        "motivosFallasBalanza": '',
        "observaciones": ''
      };

      balanzas.push(propiedades);
    }

    return balanzas;
  }


  // actualizarBalanzadas8(balanzada: Balanza78[]) {
  //   // agregado nuevo
  //   this.listadoBalanza8.push(...balanzada);

  //   if (this.listadoBalanza8 && this.listadoBalanza8.length > 0) {
  //     this.balanzas8.clear();

  //     this.listadoBalanza8.forEach(x => {
  //       this.balanzas8.push(this.initBalanzas8(x));
  //     });

  //     if (balanzada)
  //       this.balanzas78Service.setBalanza8(this.balanza8Form);
  //   }

  //   // Totalizador debajo de grilla
  //   this.resultado8 = this.agruparProductos(this.listadoBalanza8);

  //   this.verificarNombresBuque();
  // }

  actualizarBalanzadas8(balanzada: Balanza78[]) {
    // console.log('--------------- INI actualizarBalanzadas8_Prueba() --------------');
    // console.log('balanzada: ', balanzada);
    // console.log('this.balanzas8 - FormArray: ', this.balanzas8);
    // console.log('-----------------------------');

    if (!this.balanzas8 || this.balanzas8.length == 0 || this.balanzas8.value[0].id===0 ) {
      // console.log('1er vuelta - cuando FormArray esta vacio');
      this.balanzas8.clear();
      
      balanzada.forEach(x => {
        this.balanzas8.push(this.initBalanzas8(x));
      });

    } else {
      // console.log('2da vuelta');
      // console.log('this.balanzas8: ', this.balanzas8);
      
      // Filtro las balanzadas que vienen y que ya esten en la grilla, para aplicar un 'patchValue'
      // this.balanzadasFueraDeForm = [];

      let balanzadasFueraDeForm = balanzada.filter( b => this.balanzas8.value.every( bal => b.id !== bal.id ) );

      // for(let bal of this.balanzas8.value){
      //   let balanzadaEnFormArray = balanzada.find( b => b.id === bal.id );
      //   console.log('balanzadaEnFormArray: ', balanzadaEnFormArray);
        
      //   // this.balanzas8.patchValue(
      //   //   {
      //   //     ...balanzadaEnFormArray[0],
      //   //     motivoBalanza: bal.motivoBalanza, 
      //   //     observaciones: bal.observaciones 
      //   //   });
      // }

      balanzadasFueraDeForm.forEach(x => {
        this.balanzas8.push(this.initBalanzas8(x));
      });
    }
    // console.log('estado FormArray: ', this.balanzas8.value);
    // console.log('this.balanzadasEnForm: ', this.balanzadasEnForm);
    // console.log('--------------- FIN actualizarBalanzadas8_Prueba() --------------');

    if (balanzada)
        this.balanzas78Service.setBalanza8(this.balanza8Form);

    // Productos y toneladas, agrupado por producto
    this.resultado8 = this.agruparProductos(this.balanzas8.value);

    this.verificarNombresBuque();
  }

  actualizarBalanzadas7(balanzada: Balanza78[]) {

    if (!this.balanzas7 || this.balanzas7.length == 0 || this.balanzas7.value[0].id===0 ) {
      this.balanzas7.clear();
      
      balanzada.forEach(x => {
        this.balanzas7.push(this.initBalanzas7(x));
      });

    } else {
      // Filtro las balanzadas que vienen y que ya esten en la grilla, para aplicar un 'patchValue'
      let balanzadasFueraDeForm = balanzada.filter( b => this.balanzas7.value.every( bal => b.id !== bal.id ) );

      balanzadasFueraDeForm.forEach(x => {
        this.balanzas7.push(this.initBalanzas7(x));
      });
    }

    if (balanzada)
        this.balanzas78Service.setBalanza7(this.balanza7Form);

    // Productos y toneladas, agrupado por producto
    this.resultado7 = this.agruparProductos(this.balanzas7.value);

    this.verificarNombresBuque();
  }

  agruparProductos(balanza: Balanza78[]): TotToneladas[] {

    let resumen = balanza.reduce((p, c) => { // <-- primero agrupamos 
      p[c.producto] = (p[c.producto] || 0) + c.toneladas;
      return p;
    }, {});

    return Object.keys(resumen).map(e => { // <-- después transformamos el formato
      const o: any = {};
      o.producto = e;
      o.toneladas = resumen[e];
      return o;
    });
  }

  verificarNombresBuque() {
    let balanza7Diferente = this.listadoBalanza7.find(x => x.nombreBuque != this.embarque.nombreBuque);
    let balanza8Diferente = this.listadoBalanza8.find(x => x.nombreBuque != this.embarque.nombreBuque);

    if (balanza7Diferente) this.balanza7DiferenteNombre = true;
    if (balanza8Diferente) this.balanza8DiferenteNombre = true;
  }

  verificaCamposCompletos() {
    let balanza7Incompleta = this.balanzas7.value.find(x => x.toneladas < 1000 && (x.motivosFallasBalanza == null || x.observaciones == '') && x.id != 0);
    let balanza8Incompleta = this.balanzas8.value.find(x => x.toneladas < 1000 && (x.motivosFallasBalanza == null || x.observaciones == '') && x.id != 0);

    this.balanzasIncompletas = balanza7Incompleta != undefined || balanza8Incompleta != undefined ? true : false;
  }

  get balanzas7(): FormArray {
    return this.balanza7Form.get("balanzas7") as FormArray;
  }
  get balanzas8(): FormArray {
    return this.balanza8Form.get("balanzas8") as FormArray;
  }

  obtenerBalanzas7() {
    return this.balanza7Form.getRawValue().balanzas7;
  }
  obtenerBalanzas8() {
    return this.balanza8Form.getRawValue().balanzas8;
  }

  compareMotivosBalanzas(c1: MotivosFallasBalanza, c2: MotivosFallasBalanza) {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }

  guardarModal(index: number, numeroBalanza: number) {
    let balanza = 'balanzas' + numeroBalanza.toString();

    for (let i = 0; i < this[balanza].length; i++) {
      if (this[balanza].value[i].seleccionado) {
        this[balanza].controls[i].patchValue({
          motivosFallasBalanza: this[balanza].value[index].motivosFallasBalanza,
          observaciones: this[balanza].value[index].observaciones
        });
      }
    }
  }

  seleccionarTodo(numeroBalanza: number) {
    let balanza = 'balanzas' + numeroBalanza.toString();
    this.seleccionado = !this.seleccionado;

    for (let i = 0; i < this[balanza].length; i++) {
      // El cambio masivo no debería afectar a los pesajes normales.
      if (this[balanza].value[i].toneladas < 1000) {
        this[balanza].controls[i].patchValue({ seleccionado: this.seleccionado });
      }
    }
  }

  openModalAddBuque(modal: any) {
    this.errorMessage = false;
    this._modalService.open(modal);
  }

  terminarCargaExportar() {
    this.verificaCamposCompletos();
    // this.balanzasIncompletas = false; // Descomentar para probar el otro camino
    let texto = this.balanzasIncompletas ? "En las Balanzas hay campos incompletos. Para poder continuar, debe completarlos." :
      "Desea terminar la carga y exportar planillas?";

    this.confirmationDialogService.confirm('¡Atención!', texto, 'Aceptar', '', null, null, Tipoalerta.Success)
      .then((confirmed) => {
        if (confirmed && !this.balanzasIncompletas) {
          this.pasaBarcoAPostOperativo();
          this.exportarBalanzasAExcel();
        }
        else
          return;
      }).catch(() => window.location.reload());
  }

  pasaBarcoAPostOperativo() {
    this.embarqueService.actualizarEstadoBuque(this.embarqueId, 3);
  }

  exportarBalanzasAExcel(): void {
    let header = ["id", "numero", "fecha", "hora", "toneladas", "producto", "bodega", "porcentaje", "motivo", "observaciones"]
    //create new excel work book
    let workbook = new Workbook();

    // ---------- inicio BALANZA 7 ----------
    let propiedadesDeBalanza7 = this.procesarPropiedadesBalanza('7');
    //add name to sheet
    let worksheet = workbook.addWorksheet("Balanza-7");
    //add column name
    let headerRow = worksheet.addRow(header);

    for (let x1 of propiedadesDeBalanza7) {
      let x2 = Object.keys(x1);
      let temp = []
      for (let y of x2) {
        temp.push(x1[y])
      }
      worksheet.addRow(temp)
    }
    // ---------- fin BALANZA 7 ----------

    // ---------- inicio BALANZA 8 ----------
    let propiedadesDeBalanza8 = this.procesarPropiedadesBalanza('8');
    let worksheet8 = workbook.addWorksheet("Balanza-8");
    let headerRow8 = worksheet8.addRow(header);

    for (let x1 of propiedadesDeBalanza8) {
      let x2 = Object.keys(x1);
      let temp = []
      for (let y of x2) {
        temp.push(x1[y])
      }
      worksheet8.addRow(temp)
    }
    // ---------- fin BALANZA 8 ----------

    //set downloadable file name
    let fname = "Balanzas_7y8"

    //add data and file name and download
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      fs.saveAs(blob, fname + '-' + new Date().valueOf() + '.xlsx');
    });
  }

  procesarPropiedadesBalanza(blza: string) {
    let balanza = 'balanzas' + blza;

    let propiedadesDeBalanzas = this[balanza].value.map(b => {
      let siglaNombreMotivo = '';
      if (b.motivosFallasBalanza) {
        let { nombre, siglas } = b.motivosFallasBalanza;
        siglaNombreMotivo = siglas + ' - ' + nombre;
      } else siglaNombreMotivo;

      let propiedades = {
        "id": b.id,
        "numero": b.numeroBalanza,
        "fecha": b.fecha,
        "hora": b.hora,
        "toneladas": b.toneladas,
        "producto": b.producto,
        "bodega": b.bodega,
        "porcentaje": b.porcentajeCarga,
        "motivo": siglaNombreMotivo,
        "observaciones": b.observaciones
      };
      return propiedades;
    });

    return propiedadesDeBalanzas;
  }

}