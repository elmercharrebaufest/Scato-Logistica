import { Component, EventEmitter, Input, OnChanges, OnInit, Output, QueryList, SimpleChanges, ViewChildren } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EmbarqueNav } from '@ScatoModels/embarque-nav';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { ModuloDeCargaService } from '@ScatoServicios/modulo-de-carga.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-navtabs',
  templateUrl: './navtabs.component.html',
  styleUrls: ['./navtabs.component.css']
})
export class NavtabsComponent implements OnInit, OnChanges {
  @Input() mostrarBoton: boolean = false;
  @Output() showPlano = new EventEmitter<boolean>();
  @Output() changeEmbarque = new EventEmitter<any>();
  @ViewChildren('listadoBotones') listadoBotones: QueryList<any>;
  elementos: EmbarqueNav[];
  elementosSinPlano: EmbarqueNav[];
  embarqueId: number;
  errorMessage: boolean = false;
  _unsubscribe: Subject<any>;
  show: number = 7;
  desplegado: boolean = false;

  constructor(
    private _modalService: NgbModal,
    private _moduloCargaService: ModuloDeCargaService,
    private _procesoService: DatosEmbarquesProcesoService,
  ) {
    this._unsubscribe = new Subject();
    this.elementos = new Array();
    this.elementosSinPlano = new Array();
    this.elementos = this._procesoService.getEmbarquesList();
  }

  ngOnInit(): void {
    this.ordenarEmbarques();
  }

  ngOnChanges(change: SimpleChanges) {
    if (change.elementos) {
      if (this.elementos) {
        this.filtrarEmbarques();
      }
    }
  }

  ordenarEmbarques(){
    this.elementos.map( (e: any) => {
      e.orden = this.elementos.indexOf(e) + 1;
      return e;
    });
    this.filtrarEmbarques();
  }

  filtrarEmbarques() {
    this.embarqueId = this._procesoService.getEmbarqueId();
    if (this.elementos.length > 0) {
      if (this.mostrarBoton) {
        this.elementosSinPlano = this.elementos.filter(e => e.cargado.toString() === 'false');
        this.elementos = this.elementos.filter(e => e.cargado.toString() === 'true');
      }
      if (!this.embarqueId) {
        this.embarqueId = this.elementos[0].id;
        this._procesoService.setEmbarque(this.elementos[0].id);
      }
    }
  }

  ngAfterViewInit() {
    if (this.elementos.length > 0) {
      var elementoSeleccionado = document.getElementById(this.embarqueId.toString());
      if (elementoSeleccionado) elementoSeleccionado.classList.add("btn-seleccionado");
    } else {
      document.getElementById('addButton').click();
    }
    this.showPlano.emit(true);
  }

  onClickHandlerClient(elemento: EmbarqueNav) {
    var elementoDeseleccionado = document.getElementsByClassName("btn-seleccionado")[0];
    if (elementoDeseleccionado != null)
      elementoDeseleccionado.classList.remove("btn-seleccionado")
    var elementoSeleccionado = document.getElementById(elemento.id.toString());
    elementoSeleccionado.classList.add("btn-seleccionado");
    if (this._procesoService.getEmbarqueSelected() != elemento) {
      this._procesoService.setEmbarque(elemento.id);
      this.changeEmbarque.emit(true);
    }
  }

  mostrarMas() {
    this.show = this.elementos.length;
    this.desplegado = true;
  }

  mostrarMenos() {
    this.show = 7;
    this.desplegado = false;
  }

  openModalAddBuque(modal: any) {
    this.errorMessage = false;
    this._modalService.open(modal);
  }

  agregarEmbarque(embarque: string) {
    this.errorMessage = false;
    if (embarque != '0') {
      let buque = this.elementosSinPlano.find(e => e.id.toString() === embarque);
      buque.cargado = true;
      this._moduloCargaService.modificarCargadoPlanoDeCarga(buque.planoDeCargaId)
        .subscribe(
          res => {
            if (res.message === 'success') {
              this.elementos.push(buque);
              this.elementos.sort((a: any, b: any) => {
                return a.orden-b.orden;
              })
              let index = this.elementosSinPlano.indexOf(buque);
              this.elementosSinPlano.splice(index, 1);
              setTimeout(() => {
                if (this.elementos.length == 1)
                  this.onClickHandlerClient(buque);
              }, 50)
            } else {
              console.log(res.message);
            }
          }
        );
      this._modalService.dismissAll();
    } else {
      this.errorMessage = true;
    }
  }

  imgSolidoLiquido(elemento) {
    if (elemento)
      return elemento.esLiquido ? "icono-liquido" : "icono-solido";
  }
}
