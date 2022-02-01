import { Component, AfterViewInit, Input } from '@angular/core';
import { ElementoGrafico } from '@ScatoModels/elemento-grafico';
import { DatosEmbarquesProcesoService } from '@ScatoServicios/datosEmbarqueProceso.service';
import { ManosEmbarqueService } from '@ScatoServicios/manosEmbarque.service';

@Component({
  selector: 'app-grafico-carga',
  templateUrl: './grafico-carga.component.html',
  styleUrls: ['./grafico-carga.component.css']
})
export class GraficoCargaComponent implements AfterViewInit {
  datosEmbarque: any;
  toggleColor: boolean = true;
  toggleForma: string = 'rect';
  materialSeleccionado: any = null;
  hoy = new Date();
  tempDictionary: object = {};
  materialDictionary: object = {};
  lastSelectedElement: any = null;
  rotacionCheckbox: boolean = false;
  
  constructor(private _procesoService: DatosEmbarquesProcesoService,
    private _manosEmbarqueService: ManosEmbarqueService) { 
    this.makeDraggable.bind(this);
    this.datosEmbarque = this._procesoService.getDatosGrafico();
    console.log(this.datosEmbarque);
    this.initEventosManos();
  }

  ngAfterViewInit(){
    this.makeDraggable(document.getElementById('grafico-carga'));
  }

  initEventosManos(){
    this._manosEmbarqueService.removerManoDeEmbarque.subscribe(
      res => this.removerManoDeEmbarque(res.celda, res.sentido)
    );
    this._manosEmbarqueService.removerResaltadoSilos.subscribe(
      res => this.removerResaltadoSilos(res.posicion, res.esPrevious, res.cantidadEnSilo)
    );
    this._manosEmbarqueService.resaltarSilo.subscribe(
      res => this.resaltarSilo(res)
    );
    this._manosEmbarqueService.agregarManoDeEmbarque.subscribe(
      res => this.agregarManoDeEmbarque(res.celda, res.sentido)
    );
    this._manosEmbarqueService.agregarTabique.subscribe(
      res => this.agregarTabique(res.celda, res.tabiqueDesde, res.tabiqueHasta)
    );
    this._manosEmbarqueService.removerTabique.subscribe(
      res => this.removerTabique(res)
    );
  }
  

  makeDraggable(evt) {
    var svg: any = evt;
    var thisComponent = this;
    svg.addEventListener('mousedown', startDrag);
    svg.addEventListener('mousemove', drag);
    svg.addEventListener('mouseup', endDrag);
    svg.addEventListener('mouseleave', endDrag);

    svg.addEventListener('contextmenu', agregarElementoGrafico, false);

    //Eventos para mobile
    svg.addEventListener('touchstart', startDrag);
    svg.addEventListener('touchmove', drag);
    svg.addEventListener('touchend', endDrag);
    svg.addEventListener('touchleave', endDrag);
    svg.addEventListener('touchcancel', endDrag);

    var selectedElement, offset, transform, minX, maxX, minY, maxY, lastdx = 0, lastdy = 0;
    var tempIdCounter = 1;

    var lastResizeValues = {};
    function startDrag(evt) {
      if (evt.target.classList.contains('draggable')) {
        selectedElement = evt.target;
        initialiseDragging(evt);
      }else if (evt.target.classList.contains('resize-drag')) {
        selectedElement = evt.target;
        initialiseDragging(evt);
      }
      else if (evt.target.parentNode.classList.contains('draggable-group')) {
        selectedElement = evt.target.parentNode;
        initialiseDragging(evt);
      }
    }
    function initialiseDragging(evt){
      offset = getMousePosition(evt);

      //Set boundaries
      var bbox = selectedElement.getBBox();
      let svgViewBox = svg.viewBox.baseVal;
      minX = svgViewBox.x - bbox.x;
      maxX = svgViewBox.width - bbox.x - bbox.width;
      minY = svgViewBox.y - bbox.y;
      maxY = svgViewBox.height - bbox.y - bbox.height;
      // Get all the transforms currently on this element
      var transforms = selectedElement.transform.baseVal;
      // Ensure the first transform is a translate transform
      if (transforms.length === 0 ||
          transforms.getItem(0).type !== SVGTransform.SVG_TRANSFORM_TRANSLATE) {
        // Create an transform that translates by (0, 0)
        var translate = svg.createSVGTransform();
        translate.setTranslate(0, 0);
        // Add the translation to the front of the transforms list
        selectedElement.transform.baseVal.insertItemBefore(translate, 0);
      }
      // Get initial translation amount
      transform = transforms.getItem(0);
      offset.x -= transform.matrix.e;
      offset.y -= transform.matrix.f;
    }
    function drag(evt) {
      if (selectedElement) {
        evt.preventDefault();
        var coord = getMousePosition(evt);
        var dx = coord.x - offset.x;
        var dy = coord.y - offset.y;

        if (dx < minX) { dx = minX; }
        else if (dx > maxX) { dx = maxX; }
        if (dy < minY) { dy = minY; }
        else if (dy > maxY) { dy = maxY; }

        transform.setTranslate(dx, dy);
        //transform.setRotate(45, );
        if(evt.target.classList.contains('resize-drag')){
          let containerElement = evt.target.parentNode.children[0];
          let isResized = containerElement.classList.contains('resized');

           if(isResized && lastdx == 0 && lastdy == 0){
            let key = containerElement.classList[containerElement.classList.length - 1];
            let lastResize = lastResizeValues[key];
            lastdx = lastResize.dx;
            lastdy = lastResize.dy;
          }
          
          if(containerElement.tagName == "ellipse"){
            // let radiusChange = dx+dy > lastdx+lastdy ? 1.5 : -1.5;
            let radiusXChange = Number(containerElement.getAttribute('rx')) - lastdx + 20;
            containerElement.setAttribute('rx', radiusXChange + dx - 20);
            let radiusYChange = Number(containerElement.getAttribute('ry')) - lastdy + 20;
            containerElement.setAttribute('ry', radiusYChange + dy - 20);
          } else if(containerElement.tagName == "rect"){
            let initialWidth = Number(containerElement.getAttribute('width')) - lastdx + 20;
            let initialHeight = Number(containerElement.getAttribute('height')) - lastdy + 20;
            containerElement.setAttribute('width', initialWidth + dx - 20);
            containerElement.setAttribute('height', initialHeight + dy - 20);
          }
          if(!isResized){
            containerElement.classList.add('resized');
            containerElement.classList.add('resize'+Object.keys(lastResizeValues).length);
            lastResizeValues['resize'+Object.keys(lastResizeValues).length] = {dx: dx, dy: dy};
          } else if(isResized){
            let key = containerElement.classList[containerElement.classList.length - 1];
            lastResizeValues[key] = {dx: dx, dy: dy};
          }
          lastdx = dx;
          lastdy = dy;
        }
      }
    }
    function endDrag(evt) {
      selectedElement = null;
      lastdx = 0;
      lastdy = 0;
    }
    function getMousePosition(evt) {
      var CTM = svg.getScreenCTM();
      if (evt.touches) { evt = evt.touches[0]; }
      return {
        x: (evt.clientX - CTM.e) / CTM.a,
        y: (evt.clientY - CTM.f) / CTM.d
      };
    }

    function agregarElementoGrafico(evt){
      evt.preventDefault();
      let mousePos = getMousePosition(evt);
      if(thisComponent.materialSeleccionado){
        let tempId = tempIdCounter;
        tempIdCounter++;
        if(thisComponent.toggleForma == 'ellipse'){
          thisComponent.agregarCirculo(mousePos, thisComponent.materialSeleccionado?.color, thisComponent.toggleColor ? "Color" : "Prod.", tempId);
        } else if(thisComponent.toggleForma == 'rect'){
          mousePos.x = (mousePos.x - 100) > 0 ? (mousePos.x - 100) : 0;
          mousePos.y = (mousePos.y - 75) > 0 ? (mousePos.y - 75) : 0;
          thisComponent.agregarRectangulo(mousePos, thisComponent.materialSeleccionado?.color, tempId);
        }
        thisComponent.tempDictionary[tempId] = thisComponent.materialSeleccionado;
      }
    }
    // function checkIfInsideRects(){
    //   let rectCoords = [];

    //   for (let coord of rectCoords){
    //     if(isInsideCoord) return coord;
    //   }
    //   return false;
    // }
  }

  agregarTabique(celda: number, tabiqueDesde: number, tabiqueHasta: number){
    if(celda && tabiqueDesde && tabiqueHasta){
      this.removerTabique(celda);

      let tabique = document.getElementById(`celda${celda}-T${tabiqueDesde}-${tabiqueHasta}`);
      if(tabique) tabique.classList.add("tabique");
    }
  }

  removerTabique(celda: number){
    let tabiqueAnterior = document.querySelector(`.tabique[id^="celda${celda}-"]`);​
    if(tabiqueAnterior) tabiqueAnterior.classList.remove("tabique");
  }

  removerManoDeEmbarque(celda, sentido){
    if(!isNaN(celda)){
      let flechaAnterior = document.querySelector('#arrowCelda'+celda+'-'+sentido);​
      if(flechaAnterior) flechaAnterior.remove();
    }
  }

  agregarManoDeEmbarque(celda: number, sentido: number){
    if(celda && sentido && !isNaN(celda)){
      let grupoCelda = document.querySelector('#gc' + celda);

      if(grupoCelda){
        //Guia de sentidos
        //1 == Norte a sur
        //2 == Sur a norte
        //3 == Centro a sur
        //4 == Centro a norte
        //5 == Oeste a este
        //6 == Este a oeste
        //7 == Centro a este
        //8 == Centro a Oeste
        let coordenadasPorCeldaYSentido = {
          '7': {
            '5': {'x':48, 'y':15, 'z': 270},
            '6': {'x':48, 'y':340, 'z': 90},
            '7': {'x':33, 'y':170, 'z': 270},
            '8': {'x':61, 'y':170, 'z': 90}
          },
          '20': {
            '1': {'x':860, 'y':555, 'z': 0},
            '2': {'x':465, 'y':555, 'z': 180},
            '3': {'x':670, 'y':540, 'z': 0},
            '4': {'x':670, 'y':570, 'z': 180}
          },
          '23': {
            '1': {'x':390, 'y':109, 'z': 0},
            '2': {'x':-5, 'y':109, 'z': 180},
            '3': {'x':195, 'y':94, 'z': 0},
            '4': {'x':195, 'y':124, 'z': 180}
          },
          '30': {
            '1': {'x':300, 'y':109, 'z': 0},
            '2': {'x':-5, 'y':109, 'z': 180},
            '3': {'x':150, 'y':94, 'z': 0},
            '4': {'x':150, 'y':124, 'z': 180}
          },
        }

        let arrowTemplate = 
        `
          <g id="arrowCelda${celda}-${sentido}" transform="translate(${coordenadasPorCeldaYSentido[celda][sentido].x} ${coordenadasPorCeldaYSentido[celda][sentido].y}) rotate(${coordenadasPorCeldaYSentido[celda][sentido].z},33,16)">
            <use xlink:href="#myArrow"></use>
          </g>
        `;
        grupoCelda.insertAdjacentHTML('beforeend',arrowTemplate);
      }
    }
  }

  agregarElementosGraficos(elementos: ElementoGrafico[]){
    for(let elemento of elementos){
      let coordenadas = {x: elemento.x, y: elemento.y};
      this.materialDictionary[elemento.id] = elemento.materialPuerto;
      if(elemento.forma == 'ellipse'){
        this.agregarCirculo(coordenadas, elemento.materialPuerto?.color ?? '#D87621', elemento.tipo == "Prod." ? "Prod." : "Color",null , elemento.radioX, elemento.radioY, elemento.id);
      } else if(elemento.forma == 'rect'){
        this.agregarRectangulo(coordenadas, elemento.materialPuerto?.color ?? '#D87621',null, elemento.width, elemento.height, elemento.rotacion, elemento.id);
      }
    } 
  }

  agregarCirculo(mousePos, color, texto, tempId = null, radioX = null, radioY = null, id = null){
    let template = `
    <g id="${id ?? ''}" tempid="${tempId ?? ''}" class="draggable-group elementoGrafico" style="cursor: move;">
      <ellipse cx="${mousePos.x}" cy="${mousePos.y}" rx="${radioX ?? (texto == "Prod." ? 28 : 40)}" ry="${radioY ?? (texto == "Prod." ? 16 : 40)}" fill='${color}' opacity= "${texto == "Prod." ? 0.85: 0.95}"  ${texto == "Prod." ? `stroke="black" stroke-width="2"`:""} />
      <text x="${mousePos.x}" y="${mousePos.y}" 
      text-anchor="middle"
      fill="${color == '#555555' ? 'white' : 'black'}"
      alignment-baseline="middle"
      style="-webkit-touch-callout: none;-webkit-user-select: none;-khtml-user-select: none;-moz-user-select: none;-ms-user-select: none;user-select: none;"
      >${texto}</text>
      ${texto != "Prod." ? 
      `<circle class="resize-drag" cx="${mousePos.x + 50}" cy="${mousePos.y + 50}" r="5" fill='gray' style="cursor: pointer; visibility: hidden"/>`
      : ""}
    </g>
    `;
    this.agregarTemplateElemento(template, texto);
  }

  agregarRectangulo(mousePos, color = null, tempId = null, width = null, height = null, rotacion = false, id = null){
    let template = `
    <g id="${id ?? ''}" tempid="${tempId ?? ''}" class="draggable-group elementoGrafico" style="cursor: move;" ${rotacion ? `transform="translate(0 0) rotate(120 ${mousePos.x + width/2} ${mousePos.y + height/2})"`: ""}>
      <rect x="${mousePos.x}" y="${mousePos.y}" width="${width ?? 200}" height="${height ?? 150}" fill='${color ?? "#D87621"}' opacity= "0.85" rx="15" ry="15"/>
      <circle class="resize-drag" cx="${mousePos.x + 210}" cy="${mousePos.y + 160}" r="5" fill='gray' style="cursor: pointer; visibility: hidden"/>
    </g>
    `;
    this.agregarTemplateElemento(template);
  }

  agregarTemplateElemento(template, tipo = null){
    let svg = tipo != "Prod." ? document.getElementById('elementosGraficos') : document.getElementById('elementosGraficosProd');

    svg.insertAdjacentHTML('beforeend',template);
    svg.lastElementChild.addEventListener('dblclick', function(evt){
      if(thisComponent.lastSelectedElement.getAttribute('tempid') == (<HTMLElement>evt.target).getAttribute('tempid')
        || thisComponent.lastSelectedElement.id == (<HTMLElement>evt.target).id){
        thisComponent.lastSelectedElement = null;
      } 
      this.remove();
    });
    let thisComponent = this;
    svg.lastElementChild.addEventListener('mousedown', function(evt){
      let list = document.querySelectorAll(".resize-drag");
      thisComponent.lastSelectedElement = (<HTMLElement>evt.target).parentElement;
      thisComponent.rotacionCheckbox = (<HTMLElement>evt.target).parentElement.getAttribute("transform")?.includes('rotate');
      for (var i = 0; i < list.length; ++i) {
        (<HTMLElement>list[i]).style.visibility = "visible";
      }
    });
    svg.lastElementChild.addEventListener('blur', function(evt){
      let list = document.querySelectorAll(".resize-drag");
      for (var i = 0; i < list.length; ++i) {
        (<HTMLElement>list[i]).style.visibility = "hidden";
      }
    });
  }

  rotacionLastSelected(){
    if(this.lastSelectedElement){
      let gContainer = this.lastSelectedElement;
      let transforms = gContainer.transform.baseVal;
      if(this.rotacionCheckbox){
        let rect = gContainer.children[0];
        if(rect.tagName == 'rect'){
          // let coordTransform = /translate\(\s*([^\s,)]+)[ ,]([^\s,)]+)/.exec(gContainer.getAttribute('transform'));
          let svg: any = document.getElementById('grafico-carga');
          let width = rect.getAttribute('width');
          let height = rect.getAttribute('height');
          let x = Number(rect.getAttribute('x'));
          let y = Number(rect.getAttribute('y'));

          let cx = x + width/2;
          let cy = y + height/2;

          let rotate = svg.createSVGTransform();
          rotate.setRotate(120, cx, cy);

          if (transforms.length == 1 && transforms.getItem(transforms.length - 1).type !== SVGTransform.SVG_TRANSFORM_ROTATE) {
            transforms.appendItem(rotate);
          }else if (transforms.length > 0 && transforms.getItem(transforms.length - 1).type == SVGTransform.SVG_TRANSFORM_ROTATE){
            transforms.replaceItem(rotate, transforms.length - 1);
          }
        }
      } else {
        if(transforms.getItem(transforms.length - 1).type == SVGTransform.SVG_TRANSFORM_ROTATE){
          transforms.removeItem(transforms.length - 1);
        }
      }
    }
  }

  obtenerElementosGraficos() : ElementoGrafico[]{
    let elementos: ElementoGrafico[] = [];
    
    for(let item of document.querySelectorAll('.elementoGrafico')){
      let id = item.id ? item.id : null;
      let coordTransform = /translate\(\s*([^\s,)]+)[ ,]([^\s,)]+)/.exec(item.getAttribute('transform'));
      let forma = item.children[0].tagName;

      let tipo = forma == 'ellipse' ? item.children[1].innerHTML : item.children[0].classList[0];

      let width = forma == 'rect' ? item.children[0].getAttribute('width') : null;
      let height = forma == 'rect' ? item.children[0].getAttribute('height') : null;
      let radioX = forma == 'ellipse' ? item.children[0].getAttribute('rx') : null;
      let radioY = forma == 'ellipse' ? item.children[0].getAttribute('ry') : null;

      let formaX = forma == 'ellipse' ? item.children[1].getAttribute('x') : item.children[0].getAttribute('x');
      let formaY = forma == 'ellipse' ? item.children[1].getAttribute('y') : item.children[0].getAttribute('y');
      let x = coordTransform ? Number(formaX) + Number(coordTransform[1]) : Number(formaX);
      let y = coordTransform ? Number(formaY) + Number(coordTransform[2]) : Number(formaY);

      let rotacion = !!item.getAttribute('transform')?.includes('rotate');

      let materialPuerto;
      if(!id){
        let tempId = item.getAttribute('tempid');
        materialPuerto = this.tempDictionary[tempId];
      } else{
        materialPuerto = this.materialDictionary[id];
      }
      
      let elementoGrafico = new ElementoGrafico(id, tipo, x, y, forma, width, height, radioX, radioY, materialPuerto, rotacion);
      elementos.push(elementoGrafico);
    }

    console.log(elementos);
    return elementos;
  }

  limpiarGraficoCarga(){
    for(let item of document.querySelectorAll('.elementoGrafico')){
      item.remove();
    }
  }

  resaltarSilo(id){
    let silo = document.querySelector('#siloId'+id);
    if(silo) silo.classList.add('siloResaltado');
  }

  
  removerResaltadoSilos(id, esPrevious?, cantidadEnSilo?){
    let silos = document.querySelectorAll('#siloId'+id);

    if(silos.length == 0) return;
    
    let silo = document.querySelector('#siloId'+id);
    console.log('Se remueve resaltado Silo ', id);
    if(silo) silo.classList.remove('siloResaltado');
    
    // let silos = document.querySelectorAll('.silo');
    // for (var i = 0; i < silos.length; ++i) {
    //   (<HTMLElement>silos[i]).classList.remove('siloResaltado');
    // }
  }

  toggleElementoGrafico(){
    this.toggleColor = !this.toggleColor;
  }

  toggleFormaElementoGrafico(){
    if(this.toggleForma == 'ellipse'){
      this.toggleForma = 'rect';
    } else if(this.toggleForma == 'rect'){
      this.toggleForma = 'ellipse';
    }
  }

  obtenerFecha(){
    return new Date().toLocaleDateString();
  }
}
