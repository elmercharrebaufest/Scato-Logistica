import { Component, Input, OnChanges, OnInit } from '@angular/core';
import {
  CalendarEvent,
} from 'angular-calendar';
import {
  isSameDay,
  isSameMonth,
} from 'date-fns';
import { Subject } from 'rxjs';
import { InstanciaWorkflowPuerto } from '@ScatoModels/instancia-wokflow-puerto';

const colors: any = {
  sanbenito: {
    primary: '#3376A1',
    secondary: '#3376A1B3',
  },
  vicentin: {
    primary: '#FEBE2C',
    secondary: '#FEBE2CB3',
  },
  nouryon: {
    primary: '#FE521A',
    secondary: '#FE521AB3',
  },
  otromuelle: {
    primary: '#666666',
    secondary: '#666666B3',
  },
};

@Component({
  selector: 'app-lineup-calendario',
  templateUrl: './lineup-calendario.component.html',
  styleUrls: ['./lineup-calendario.component.css']
})
export class LineupCalendarioComponent implements OnInit, OnChanges {
  @Input() listadoEmbarques: InstanciaWorkflowPuerto[];
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();
  events: CalendarEvent[] = [];
  activeDayIsOpen: boolean = false;
  view: string = 'month'
  constructor() { }


  ngOnChanges() {
    console.log('actualizando calendario');
    this.cargar();
    this.refresh.next();
  } 

  ngOnInit(): void {
    this.cargar();
  }

  cargar() {

    this.events = this.listadoEmbarques.map(x =>
      ({
        start: new Date(x.embarque.fechaRecalada),
        title: x.embarque.nombreBuque,
        instanciaWorkflow: x,
        color: x.embarque.sanBenito ? colors.sanbenito : 
              x.embarque.vicentin ? colors.vicentin :
              x.embarque.noryon ? colors.nouryon : colors.otromuelle,
        forecolor: x.embarque.vicentin ? '#000000' : '#ffffff',
        dia: new Date(),
        allDay: true
      }));
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  colorDeFondo(event){
    return event.start.setHours(0,0,0,0) >= event.dia.setHours(0,0,0,0)
           ? event.color.primary : event.color.secondary
  }
}
